using System;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using ScottPlot;
using ScottPlot.WinForms;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        private readonly FormsPlot _torPlot = new FormsPlot();

        // Kullanıcı grafiği incelemek için zoom/pan yapınca eksenleri sabitle
        private bool _torViewFrozen = false;

        // X ekseni başlangıç zamanı
        private DateTime _torStartTime;
        private bool _plotReady;

        private const int PlotPollingIntervalMs = 500;
        private const int PlotRetentionSeconds = 24 * 60 * 60; // 24 saat
        private const int SharedCapacity = (PlotRetentionSeconds * 1000) / PlotPollingIntervalMs;
        private const int MaxRenderPoints = 2400;

        // Ring buffer (500ms polling için 24 saat)
        private const int Capacity = SharedCapacity;
        private readonly double[] _x = new double[Capacity];
        private readonly double[] _yRaw = new double[Capacity];
        private int _count, _head;

        private readonly double[] _resX = new double[Capacity];
        private readonly double[] _resY = new double[Capacity];
        private int _resCount, _resHead;

        // Modlar
        private bool _logViewEnabled = false;
        private bool _torAllTimeView = false;

        private bool _torUiWired;
        private bool _exportWired;

        private void InitTorPlotOnPanel()
        {
            if (_plotReady) return;
            _plotReady = true;

            _torStartTime = DateTime.Now;

            _torPlot.Dock = DockStyle.Fill;
            panelTorque.Controls.Add(_torPlot);

            ApplyTorPlotStyle();
            WireTorPlotUiControls();

            // Zoom/pan -> freeze
            _torPlot.MouseDown += (_, __) =>
            {
                _torViewFrozen = true;
                UpdateTorOverlayUi();
            };

            _torPlot.MouseWheel += (_, __) =>
            {
                _torViewFrozen = true;
                UpdateTorOverlayUi();
            };

            // Double click -> LIVE(2m)
            _torPlot.DoubleClick += (_, __) =>
            {
                _torViewFrozen = false;
                _torAllTimeView = false;
                UpdateTorOverlayUi();
                TorPlot_Refresh(120);
            };

            _torPlot.Refresh();
        }

        private void WireTorPlotUiControls()
        {
            if (_torUiWired) return;
            _torUiWired = true;

            WirePlotExportButton();

            // Label stil
            lblTorFrozen.Text = "🔒 FROZEN";
            lblTorFrozen.AutoSize = true;
            lblTorFrozen.BackColor = System.Drawing.Color.FromArgb(200, 220, 0, 0);
            lblTorFrozen.ForeColor = System.Drawing.Color.White;
            lblTorFrozen.Padding = new Padding(6, 3, 6, 3);
            lblTorFrozen.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            UpdateTorOverlayUi();

            btnTorLive.Click += (_, __) =>
            {
                _torViewFrozen = false;
                _torAllTimeView = false;
                UpdateTorOverlayUi();
                TorPlot_Refresh(120); // ✅ son 2dk X + Y autoscale (aşağıda)
            };

            btnTorAll.Click += (_, __) =>
            {
                _torViewFrozen = false;
                _torAllTimeView = true;
                UpdateTorOverlayUi();
                TorPlot_Refresh(null);
            };

            btnTorFreeze.Click += (_, __) =>
            {
                _torViewFrozen = !_torViewFrozen;
                UpdateTorOverlayUi();

                if (!_torViewFrozen)
                    TorPlot_Refresh(_torAllTimeView ? (int?)null : 120);
            };

            chkTorLog.CheckedChanged += (_, __) =>
            {
                _logViewEnabled = chkTorLog.Checked;
                UpdateTorOverlayUi();
                TorPlot_Refresh(_torAllTimeView ? (int?)null : 120);
            };
        }


        private void WirePlotExportButton()
        {
            if (_exportWired) return;
            _exportWired = true;
            btnExport.Click += (_, __) => ExportAllPlotsToExcel();
        }

        private void ExportAllPlotsToExcel()
        {
            using var sfd = new SaveFileDialog
            {
                Title = "Export plots to Excel",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                AddExtension = true,
                FileName = $"plots_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (sfd.ShowDialog(this) != DialogResult.OK)
            {
                PostStatus("Excel export canceled.");
                return;
            }

            try
            {
                var (torTime, torValues) = GetOrderedSeries(_torStartTime, _x, _yRaw, _count, _head, Capacity);
                var (resTime, resValues) = GetOrderedSeries(_torStartTime, _resX, _resY, _resCount, _resHead, Capacity);
                var alignedResistance = AlignSeriesByLastKnown(torTime, resTime, resValues);
                var (rateTime, rateValues) = GetOrderedSeries(_rateStartTime, _rateX, _rateY, _rateCount, _rateHead, RateCapacity);
                var (thkTime, thkValues) = GetOrderedSeries(_thkStartTime, _thkX, _thkY, _thkCount, _thkHead, ThkCapacity);

                using var workbook = new XLWorkbook();

                var wsTor = workbook.Worksheets.Add("TOR");
                wsTor.Cell(1, 1).Value = "Timestamp";
                wsTor.Cell(1, 2).Value = "TOR (Torr)";
                wsTor.Cell(1, 3).Value = "TOR Log10(Torr)";
                wsTor.Cell(1, 4).Value = "Rezistance Temperature";

                for (int i = 0; i < torValues.Length; i++)
                {
                    int row = i + 2;
                    wsTor.Cell(row, 1).Value = torTime[i];
                    wsTor.Cell(row, 2).Value = torValues[i];
                    wsTor.Cell(row, 3).Value = Math.Log10(Math.Max(torValues[i], 1e-12));

                    if (alignedResistance[i].HasValue)
                        wsTor.Cell(row, 4).Value = alignedResistance[i].Value;
                }

                wsTor.Column(1).Style.DateFormat.Format = "yyyy-mm-dd hh:mm:ss.000";
                wsTor.Columns().AdjustToContents();

                var wsRate = workbook.Worksheets.Add("Rate");
                wsRate.Cell(1, 1).Value = "Timestamp";
                wsRate.Cell(1, 2).Value = "Rate (Å/s)";

                for (int i = 0; i < rateValues.Length; i++)
                {
                    int row = i + 2;
                    wsRate.Cell(row, 1).Value = rateTime[i];
                    wsRate.Cell(row, 2).Value = rateValues[i];
                }

                wsRate.Column(1).Style.DateFormat.Format = "yyyy-mm-dd hh:mm:ss.000";
                wsRate.Columns().AdjustToContents();

                var wsThickness = workbook.Worksheets.Add("Thickness");
                wsThickness.Cell(1, 1).Value = "Timestamp";
                wsThickness.Cell(1, 2).Value = "Thickness (kÅ)";

                for (int i = 0; i < thkValues.Length; i++)
                {
                    int row = i + 2;
                    wsThickness.Cell(row, 1).Value = thkTime[i];
                    wsThickness.Cell(row, 2).Value = thkValues[i];
                }

                wsThickness.Column(1).Style.DateFormat.Format = "yyyy-mm-dd hh:mm:ss.000";
                wsThickness.Columns().AdjustToContents();

                workbook.SaveAs(sfd.FileName);
                PostStatus($"Excel export completed: {sfd.FileName}");
            }
            catch (Exception ex)
            {
                PostStatus($"Excel export failed: {ex.Message}");
            }
        }

        private void UpdateTorOverlayUi()
        {
            // ✅ LIVE(2m) asla disable olmasın
            btnTorLive.Enabled = true;

            // ALL sadece live moddayken aktif kalsın (istersen bunu da hep aktif yapabiliriz)
            btnTorAll.Enabled = true;

            btnTorFreeze.Text = _torViewFrozen ? "FROZEN" : "FREEZE";
            lblTorFrozen.Visible = _torViewFrozen;

            if (chkTorLog.Checked != _logViewEnabled)
                chkTorLog.Checked = _logViewEnabled;
        }

        private void ApplyTorPlotStyle()
        {
            _torPlot.Plot.Axes.Left.Label.Text = "Pressure (Torr)";
            _torPlot.Plot.Axes.Right.Label.Text = "Rezistance Temp (°C)";
            _torPlot.Plot.Axes.Right.IsVisible = true;
            var xticks = new ScottPlot.TickGenerators.NumericAutomatic()
            {
                LabelFormatter = pos =>
                    TimeSpan.FromSeconds(pos).ToString(@"hh\:mm\:ss")
            };
            _torPlot.Plot.Axes.Bottom.TickGenerator = xticks;

            var yticks = new ScottPlot.TickGenerators.NumericAutomatic()
            {
                LabelFormatter = pos =>
                {
                    if (!_logViewEnabled)
                        return pos.ToString("G4");

                    double torr = Math.Pow(10, pos);
                    return torr.ToString("0.###E+0");
                }
            };
            _torPlot.Plot.Axes.Left.TickGenerator = yticks;

            var rightTicks = new ScottPlot.TickGenerators.NumericAutomatic()
            {
                LabelFormatter = pos => pos.ToString("0.##")
            };
            _torPlot.Plot.Axes.Right.TickGenerator = rightTicks;
        }

        public void TorPlot_AddPoint(DateTime time, double torTorr, int? showLastSeconds = 120)
        {
            double elapsed = (time - _torStartTime).TotalSeconds;
            torTorr = Math.Max(torTorr, 1e-12);

            _x[_head] = elapsed;
            _yRaw[_head] = torTorr;

            _head = (_head + 1) % Capacity;
            if (_count < Capacity) _count++;

            int? seconds = showLastSeconds;
            if (showLastSeconds == 120)
                seconds = _torAllTimeView ? null : 120;

            TorPlot_Refresh(seconds);
        }

        public void TorPlot_AddResistancePoint(DateTime time, double resistanceValue, int? showLastSeconds = 120)
        {
            if (!_plotReady)
                InitTorPlotOnPanel();

            double elapsed = (time - _torStartTime).TotalSeconds;

            _resX[_resHead] = elapsed;
            _resY[_resHead] = resistanceValue;

            _resHead = (_resHead + 1) % Capacity;
            if (_resCount < Capacity) _resCount++;

            int? seconds = showLastSeconds;
            if (showLastSeconds == 120)
                seconds = _torAllTimeView ? null : 120;

            TorPlot_Refresh(seconds);
        }

        private (double[] x, double[] yRaw) GetTorView(int? showLastSeconds, int maxPoints = MaxRenderPoints)
            => BuildViewData(_x, _yRaw, _count, _head, Capacity, showLastSeconds, maxPoints);

        private void TorPlot_Refresh(int? showLastSeconds)
        {
            var (xo, yoRaw) = GetTorView(showLastSeconds);
            var (resXo, resYo) = BuildViewData(_resX, _resY, _resCount, _resHead, Capacity, showLastSeconds, MaxRenderPoints);
            if (xo.Length == 0 && resXo.Length == 0) return;

            // Freeze modunda kullanıcı limits'ini koru
            var prevLimits = _torPlot.Plot.Axes.GetLimits();

            // Plot Y (raw veya log10(raw))
            double[] yoPlot = _logViewEnabled
                ? yoRaw.Select(v => Math.Log10(Math.Max(v, 1e-12))).ToArray()
                : yoRaw;

            _torPlot.Plot.Clear();
            ApplyTorPlotStyle();

            if (xo.Length > 0)
            {
                var torLine = _torPlot.Plot.Add.Scatter(xo, yoPlot);
                torLine.LineWidth = 2;
                torLine.Color = Colors.Blue;
                torLine.LegendText = "TOR";
            }

            if (resXo.Length > 0)
            {
                var resistanceLine = _torPlot.Plot.Add.Scatter(resXo, resYo);
                resistanceLine.LineWidth = 2;
                resistanceLine.Color = Colors.DarkRed;
                resistanceLine.LegendText = "Temp";
                resistanceLine.Axes.YAxis = _torPlot.Plot.Axes.Right;
            }

            _torPlot.Plot.Legend.IsVisible = true;

            if (_torViewFrozen)
            {
                _torPlot.Plot.Axes.SetLimits(prevLimits);
                _torPlot.Refresh();
                return;
            }

            // -------------------------
            // X ve Y limit stratejisi
            // -------------------------

            if (showLastSeconds.HasValue && showLastSeconds.Value > 0)
            {
                // ✅ X: son N saniye penceresi
                double xMax = 0;
                if (xo.Length > 0) xMax = Math.Max(xMax, xo[^1]);
                if (resXo.Length > 0) xMax = Math.Max(xMax, resXo[^1]);

                double xMin = Math.Max(0, xMax - showLastSeconds.Value);
                _torPlot.Plot.Axes.SetLimitsX(xMin, xMax);

                // ✅ Y: SADECE bu penceredeki dataya göre autoscale
                if (yoPlot.Length > 0)
                {
                    double yMin = yoPlot.Min();
                    double yMax = yoPlot.Max();

                    // çok düz çizgi ise küçük padding
                    if (Math.Abs(yMax - yMin) < 1e-12)
                    {
                        yMin -= 0.5;
                        yMax += 0.5;
                    }
                    else
                    {
                        // %5 padding (okunaklı)
                        double pad = (yMax - yMin) * 0.05;
                        yMin -= pad;
                        yMax += pad;
                    }

                    _torPlot.Plot.Axes.SetLimitsY(yMin, yMax);
                }

                if (resYo.Length > 0)
                {
                    double yMin = resYo.Min();
                    double yMax = resYo.Max();

                    if (Math.Abs(yMax - yMin) < 1e-12)
                    {
                        yMin -= 0.5;
                        yMax += 0.5;
                    }
                    else
                    {
                        double pad = (yMax - yMin) * 0.05;
                        yMin -= pad;
                        yMax += pad;
                    }

                    _torPlot.Plot.Axes.SetLimitsY(yMin, yMax, _torPlot.Plot.Axes.Right);
                }
            }
            else
            {
                // ✅ All-time: tam autoscale
                _torPlot.Plot.Axes.AutoScale();

                if (resYo.Length > 0)
                {
                    double yMin = resYo.Min();
                    double yMax = resYo.Max();
                    if (Math.Abs(yMax - yMin) < 1e-12)
                    {
                        yMin -= 0.5;
                        yMax += 0.5;
                    }
                    else
                    {
                        double pad = (yMax - yMin) * 0.05;
                        yMin -= pad;
                        yMax += pad;
                    }

                    _torPlot.Plot.Axes.SetLimitsY(yMin, yMax, _torPlot.Plot.Axes.Right);
                }
            }

            _torPlot.Refresh();
        }



        // ===================== RATE (STM2) PLOT =====================

        private readonly FormsPlot _ratePlot = new FormsPlot();

        private bool _rateViewFrozen = false;
        private bool _rateAllTimeView = false;   // true = ALL, false = LIVE(2m)
        private bool _ratePlotReady = false;
        private bool _rateUiWired = false;

        private DateTime _rateStartTime;

        // 500ms polling için 24 saat buffer
        private const int RateCapacity = SharedCapacity;
        private readonly double[] _rateX = new double[RateCapacity];
        private readonly double[] _rateY = new double[RateCapacity];
        private int _rateCount, _rateHead;

        private void InitRatePlotOnPanel()
        {
            if (_ratePlotReady) return;
            _ratePlotReady = true;

            _rateStartTime = DateTime.Now;

            _ratePlot.Dock = DockStyle.Fill;
            panelRate.Controls.Add(_ratePlot);

            ApplyRatePlotStyle();
            WireRatePlotUiControls();

            _ratePlot.MouseDown += (_, __) =>
            {
                _rateViewFrozen = true;
                UpdateRateOverlayUi();
            };

            _ratePlot.MouseWheel += (_, __) =>
            {
                _rateViewFrozen = true;
                UpdateRateOverlayUi();
            };

            _ratePlot.DoubleClick += (_, __) =>
            {
                _rateViewFrozen = false;
                _rateAllTimeView = false;
                UpdateRateOverlayUi();
                RatePlot_Refresh(120);
            };

            _ratePlot.Refresh();
        }

        private void ApplyRatePlotStyle()
        {
            _ratePlot.Plot.Axes.Left.Label.Text = "Rate (Å/s)";

            var xticks = new ScottPlot.TickGenerators.NumericAutomatic()
            {
                LabelFormatter = pos => TimeSpan.FromSeconds(pos).ToString(@"hh\:mm\:ss")
            };
            _ratePlot.Plot.Axes.Bottom.TickGenerator = xticks;

            var yticks = new ScottPlot.TickGenerators.NumericAutomatic()
            {
                LabelFormatter = pos => pos.ToString("0.###")
            };
            _ratePlot.Plot.Axes.Left.TickGenerator = yticks;
        }

        private void WireRatePlotUiControls()
        {
            if (_rateUiWired) return;
            _rateUiWired = true;

            lblRateFrozen.Text = "🔒 FROZEN";
            lblRateFrozen.AutoSize = true;
            lblRateFrozen.BackColor = System.Drawing.Color.FromArgb(200, 220, 0, 0);
            lblRateFrozen.ForeColor = System.Drawing.Color.White;
            lblRateFrozen.Padding = new Padding(6, 3, 6, 3);
            lblRateFrozen.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            UpdateRateOverlayUi();

            btnRateLive.Click += (_, __) =>
            {
                _rateViewFrozen = false;
                _rateAllTimeView = false;
                UpdateRateOverlayUi();
                RatePlot_Refresh(120);
            };

            btnRateAll.Click += (_, __) =>
            {
                _rateViewFrozen = false;
                _rateAllTimeView = true;
                UpdateRateOverlayUi();
                RatePlot_Refresh(null);
            };

            btnRateFreeze.Click += (_, __) =>
            {
                _rateViewFrozen = !_rateViewFrozen;
                UpdateRateOverlayUi();

                if (!_rateViewFrozen)
                    RatePlot_Refresh(_rateAllTimeView ? (int?)null : 120);
            };
        }

        private void UpdateRateOverlayUi()
        {
            btnRateLive.Enabled = true;
            btnRateAll.Enabled = true;

            btnRateFreeze.Text = _rateViewFrozen ? "FROZEN" : "FREEZE";
            lblRateFrozen.Visible = _rateViewFrozen;
        }

        public void RatePlot_AddPoint(DateTime time, double rateValue, int? showLastSeconds = 120)
        {
            if (!_ratePlotReady)
                InitRatePlotOnPanel();

            double elapsed = (time - _rateStartTime).TotalSeconds;

            _rateX[_rateHead] = elapsed;
            _rateY[_rateHead] = rateValue;

            _rateHead = (_rateHead + 1) % RateCapacity;
            if (_rateCount < RateCapacity) _rateCount++;

            int? seconds = showLastSeconds;
            if (showLastSeconds == 120)
                seconds = _rateAllTimeView ? null : 120;

            RatePlot_Refresh(seconds);
        }

        private (double[] x, double[] y) GetRateView(int? showLastSeconds, int maxPoints = MaxRenderPoints)
            => BuildViewData(_rateX, _rateY, _rateCount, _rateHead, RateCapacity, showLastSeconds, maxPoints);

        private void RatePlot_Refresh(int? showLastSeconds)
        {
            var (xo, yo) = GetRateView(showLastSeconds);
            if (xo.Length == 0) return;
            var prevLimits = _ratePlot.Plot.Axes.GetLimits();

            _ratePlot.Plot.Clear();
            ApplyRatePlotStyle();
            _ratePlot.Plot.Add.Scatter(xo, yo).LineWidth = 2;

            if (_rateViewFrozen)
            {
                _ratePlot.Plot.Axes.SetLimits(prevLimits);
                _ratePlot.Refresh();
                return;
            }

            if (showLastSeconds.HasValue && xo.Length >= 2)
            {
                double xMax = xo[^1];
                double xMin = Math.Max(0, xMax - showLastSeconds.Value);
                _ratePlot.Plot.Axes.SetLimitsX(xMin, xMax);

                if (yo.Length > 0)
                {
                    double yMin = yo.Min();
                    double yMax = yo.Max();

                    if (Math.Abs(yMax - yMin) < 1e-12)
                    {
                        yMin -= 0.5;
                        yMax += 0.5;
                    }
                    else
                    {
                        double pad = (yMax - yMin) * 0.05;
                        yMin -= pad;
                        yMax += pad;
                    }

                    _ratePlot.Plot.Axes.SetLimitsY(yMin, yMax);
                }
            }
            else
            {
                _ratePlot.Plot.Axes.AutoScale();
            }

            _ratePlot.Refresh();
        }




        // ===================== THICKNESS PLOT =====================

        private readonly FormsPlot _thkPlot = new FormsPlot();

        private bool _thkViewFrozen = false;
        private bool _thkAllTimeView = false;
        private bool _thkPlotReady = false;
        private bool _thkUiWired = false;

        private DateTime _thkStartTime;

        private const int ThkCapacity = SharedCapacity;
        private readonly double[] _thkX = new double[ThkCapacity];
        private readonly double[] _thkY = new double[ThkCapacity];
        private int _thkCount, _thkHead;

        private double? _thkTargetA = null;   // Å
        private bool _thkTargetReached = false;

        private ScottPlot.Plottables.HorizontalLine? _thkTargetLine;
        private ScottPlot.Plottables.Text? _thkTargetText;



        private void InitThicknessPlotOnPanel()
        {
            if (_thkPlotReady) return;
            _thkPlotReady = true;

            _thkStartTime = DateTime.Now;

            _thkPlot.Dock = DockStyle.Fill;
            panelThickness.Controls.Add(_thkPlot);

            ApplyThicknessPlotStyle();
            WireThicknessPlotUiControls();

            _thkPlot.MouseDown += (_, __) =>
            {
                _thkViewFrozen = true;
                UpdateThicknessOverlayUi();
            };

            _thkPlot.MouseWheel += (_, __) =>
            {
                _thkViewFrozen = true;
                UpdateThicknessOverlayUi();
            };

            _thkPlot.DoubleClick += (_, __) =>
            {
                _thkViewFrozen = false;
                _thkAllTimeView = false;
                UpdateThicknessOverlayUi();
                ThicknessPlot_Refresh(120);
            };

            _thkPlot.Refresh();
        }

        private void ApplyThicknessPlotStyle()
        {
            _thkPlot.Plot.Axes.Left.Label.Text = "Thickness (kÅ)";

            var xticks = new ScottPlot.TickGenerators.NumericAutomatic()
            {
                LabelFormatter = pos => TimeSpan.FromSeconds(pos).ToString(@"hh\:mm\:ss")
            };
            _thkPlot.Plot.Axes.Bottom.TickGenerator = xticks;

            var yticks = new ScottPlot.TickGenerators.NumericAutomatic()
            {
                LabelFormatter = pos => pos.ToString("0.##")
            };
            _thkPlot.Plot.Axes.Left.TickGenerator = yticks;
        }

        private void WireThicknessPlotUiControls()
        {
            if (_thkUiWired) return;
            _thkUiWired = true;

            lblThicknessFrozen.Text = "🔒 FROZEN";
            lblThicknessFrozen.AutoSize = true;
            lblThicknessFrozen.BackColor = System.Drawing.Color.FromArgb(200, 220, 0, 0);
            lblThicknessFrozen.ForeColor = System.Drawing.Color.White;
            lblThicknessFrozen.Padding = new Padding(6, 3, 6, 3);
            lblThicknessFrozen.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            UpdateThicknessOverlayUi();

            btnThicknessLive.Click += (_, __) =>
            {
                _thkViewFrozen = false;
                _thkAllTimeView = false;
                UpdateThicknessOverlayUi();
                ThicknessPlot_Refresh(120);
            };

            btnThicknessAll.Click += (_, __) =>
            {
                _thkViewFrozen = false;
                _thkAllTimeView = true;
                UpdateThicknessOverlayUi();
                ThicknessPlot_Refresh(null);
            };

            btnThicknessFreeze.Click += (_, __) =>
            {
                _thkViewFrozen = !_thkViewFrozen;
                UpdateThicknessOverlayUi();

                if (!_thkViewFrozen)
                    ThicknessPlot_Refresh(_thkAllTimeView ? (int?)null : 120);
            };
        }

        private void UpdateThicknessOverlayUi()
        {
            btnThicknessLive.Enabled = true;
            btnThicknessAll.Enabled = true;

            btnThicknessFreeze.Text = _thkViewFrozen ? "FROZEN" : "FREEZE";
            lblThicknessFrozen.Visible = _thkViewFrozen;
        }

        public void ThicknessPlot_AddPoint(DateTime time, double thickness)
        {
            if (!_thkPlotReady)
                InitThicknessPlotOnPanel();

            double elapsed = (time - _thkStartTime).TotalSeconds;

            _thkX[_thkHead] = elapsed;
            _thkY[_thkHead] = thickness;

            _thkHead = (_thkHead + 1) % ThkCapacity;
            if (_thkCount < ThkCapacity) _thkCount++;

            int? seconds = _thkAllTimeView ? null : 120;

            ThicknessPlot_Refresh(seconds);
        }

        private void ThicknessPlot_Refresh(int? showLastSeconds)
        {
            var (xo, yo) = GetThicknessView(showLastSeconds);
            if (xo.Length == 0) return;
            var prevLimits = _thkPlot.Plot.Axes.GetLimits();

            _thkPlot.Plot.Clear();
            ApplyThicknessPlotStyle();
            _thkPlot.Plot.Add.Scatter(xo, yo).LineWidth = 2;
            // ✅ Target line (FinalThickness)
            // ✅ Target line + label (ScottPlot 5: HorizontalLine + Text annotation)
            if (_thkTargetA.HasValue)
            {
                // renk ve metin
                var lineColor = _thkTargetReached ? ScottPlot.Colors.LimeGreen : ScottPlot.Colors.Orange;
                string text = _thkTargetReached
                    ? $"Reached: {_thkTargetA.Value:0.##} kÅ"
                    : $"Target: {_thkTargetA.Value:0.##} kÅ";

                // çizgi
                _thkTargetLine = _thkPlot.Plot.Add.HorizontalLine(_thkTargetA.Value);
                _thkTargetLine.LineWidth = 2;
                _thkTargetLine.Color = lineColor;

                // yazı: sağ üst civarı (xMax, targetY) noktasına koyuyoruz
                double xLabel = (xo.Length > 0) ? xo[^1] : 0;

                _thkTargetText = _thkPlot.Plot.Add.Text(text, xLabel, _thkTargetA.Value);
                _thkTargetText.Alignment = Alignment.UpperRight;
                _thkTargetText.Color = lineColor;
            }




            if (_thkViewFrozen)
            {
                _thkPlot.Plot.Axes.SetLimits(prevLimits);
                _thkPlot.Refresh();
                return;
            }

            if (showLastSeconds.HasValue && xo.Length >= 2)
            {
                double xMax = xo[^1];
                double xMin = Math.Max(0, xMax - showLastSeconds.Value);
                _thkPlot.Plot.Axes.SetLimitsX(xMin, xMax);

                if (yo.Length > 0)
                {
                    double yMin = yo.Min();
                    double yMax = yo.Max();

                    if (Math.Abs(yMax - yMin) < 1e-12)
                    {
                        yMin -= 1;
                        yMax += 1;
                    }
                    else
                    {
                        double pad = (yMax - yMin) * 0.05;
                        yMin -= pad;
                        yMax += pad;
                    }

                    _thkPlot.Plot.Axes.SetLimitsY(yMin, yMax);
                }
            }
            else
            {
                _thkPlot.Plot.Axes.AutoScale();
            }

            _thkPlot.Refresh();
        }

        private (double[] x, double[] y) GetThicknessView(int? showLastSeconds, int maxPoints = MaxRenderPoints)
            => BuildViewData(_thkX, _thkY, _thkCount, _thkHead, ThkCapacity, showLastSeconds, maxPoints);

        private void ThicknessPlot_SetTarget(double? targetA)
        {
            _thkTargetA = targetA;
            _thkTargetReached = false; // yeni layer -> reset

            // çizgiyi bir sonraki refresh’te doğru çiz
            if (!_thkViewFrozen)
                ThicknessPlot_Refresh(_thkAllTimeView ? (int?)null : 120);
        }

        private void ThicknessPlot_SetTargetReached(bool reached)
        {
            _thkTargetReached = reached;

            // freeze değilsek anında güncelle
            if (!_thkViewFrozen)
                ThicknessPlot_Refresh(_thkAllTimeView ? (int?)null : 120);
        }

        private static (double[] x, double[] y) BuildViewData(
            double[] xBuffer,
            double[] yBuffer,
            int count,
            int head,
            int capacity,
            int? showLastSeconds,
            int maxPoints)
        {
            if (count <= 0)
                return (Array.Empty<double>(), Array.Empty<double>());

            int BufferIndex(int offset)
            {
                if (count < capacity)
                    return offset;

                return (head + offset) % capacity;
            }

            double XAt(int offset) => xBuffer[BufferIndex(offset)];

            int endOffset = count - 1;
            int startOffset = 0;

            if (showLastSeconds.HasValue && showLastSeconds.Value > 0)
            {
                double xMin = Math.Max(0, XAt(endOffset) - showLastSeconds.Value);
                startOffset = endOffset;

                while (startOffset > 0 && XAt(startOffset - 1) >= xMin)
                    startOffset--;
            }

            int pointsInRange = endOffset - startOffset + 1;
            int step = Math.Max(1, (int)Math.Ceiling(pointsInRange / (double)Math.Max(1, maxPoints)));

            int estimated = (pointsInRange + step - 1) / step;
            double[] xOut = new double[estimated + 1];
            double[] yOut = new double[estimated + 1];
            int n = 0;

            for (int offset = startOffset; offset <= endOffset; offset += step)
            {
                int idx = BufferIndex(offset);
                xOut[n] = xBuffer[idx];
                yOut[n] = yBuffer[idx];
                n++;
            }

            int lastIdx = BufferIndex(endOffset);
            if (n == 0 || xOut[n - 1] != xBuffer[lastIdx])
            {
                xOut[n] = xBuffer[lastIdx];
                yOut[n] = yBuffer[lastIdx];
                n++;
            }

            if (n == xOut.Length)
                return (xOut, yOut);

            return (xOut.Take(n).ToArray(), yOut.Take(n).ToArray());
        }



        private static double?[] AlignSeriesByLastKnown(DateTime[] targetTime, DateTime[] sourceTime, double[] sourceValues)
        {
            double?[] aligned = new double?[targetTime.Length];
            if (targetTime.Length == 0 || sourceTime.Length == 0 || sourceValues.Length == 0)
                return aligned;

            int src = 0;
            double? lastKnown = null;

            for (int i = 0; i < targetTime.Length; i++)
            {
                while (src < sourceTime.Length && sourceTime[src] <= targetTime[i])
                {
                    lastKnown = sourceValues[src];
                    src++;
                }

                aligned[i] = lastKnown;
            }

            return aligned;
        }

        private static (DateTime[] time, double[] values) GetOrderedSeries(
            DateTime startTime,
            double[] xBuffer,
            double[] yBuffer,
            int count,
            int head,
            int capacity)
        {
            if (count <= 0)
                return (Array.Empty<DateTime>(), Array.Empty<double>());

            int BufferIndex(int offset)
            {
                if (count < capacity)
                    return offset;

                return (head + offset) % capacity;
            }

            DateTime[] orderedTime = new DateTime[count];
            double[] orderedValues = new double[count];

            for (int i = 0; i < count; i++)
            {
                int idx = BufferIndex(i);
                orderedTime[i] = startTime.AddSeconds(xBuffer[idx]);
                orderedValues[i] = yBuffer[idx];
            }

            return (orderedTime, orderedValues);
        }









        // ------------------- TOR mini sparkline ------------------

        private readonly ScottPlot.WinForms.FormsPlot _torMiniPlot = new();
        private bool _torMiniInit;

        private void InitTorMiniSparkline()
        {
            if (_torMiniInit) return;
            _torMiniInit = true;

            _torMiniPlot.Dock = DockStyle.Fill;
            panelTorMini.Controls.Add(_torMiniPlot);

            var plt = _torMiniPlot.Plot;
            plt.Clear();

            // ✅ Sparkline: her şeyi kapat
            plt.Grid.IsVisible = false;
            plt.Axes.Bottom.IsVisible = false;
            plt.Axes.Left.IsVisible = false;

            // ✅ Kenar boşluklarını minimuma çek
            plt.Axes.Margins(0, 0);

            // ✅ Frame/padding’leri daha da azalt (ScottPlot 5’te etkili)
            plt.Layout.Fixed(new ScottPlot.PixelPadding(0, 0, 0, 0));

            // ✅ Arka plan
            plt.FigureBackground.Color = ScottPlot.Colors.Transparent;
            plt.DataBackground.Color = ScottPlot.Colors.Transparent;

            // ✅ Çerçeve/outline çizgilerini kapat (o “siyah çizgi” çoğunlukla bu)
            plt.HideAxesAndGrid();

            _torMiniPlot.Refresh();
        }

        private void TorMiniSparkline_Refresh(int lastSeconds = 10, bool logVisual = true)
        {
            if (!_torMiniInit) return;

            var (xo, yoTorr) = GetTorView(lastSeconds, maxPoints: 240);
            if (xo.Length < 2) return;

            double tMax = xo[^1];
            double tMin = Math.Max(0, tMax - lastSeconds);

            double[] x = xo;
            double[] yTorr = yoTorr;

            double[] y = yTorr;

            // ✅ Vakum trendi okunur olsun diye log görsel hile
            if (logVisual)
                y = y.Select(v => Math.Log10(Math.Max(v, 1e-12))).ToArray();

            var plt = _torMiniPlot.Plot;
            plt.Clear();

            var sig = plt.Add.Scatter(x, y);
            sig.LineWidth = 2;

            // ✅ ScottPlot renk
            sig.Color = GetTorSparklineColor(_lastTorTorr ?? 0);

            // ✅ X penceresi sabit
            plt.Axes.SetLimitsX(tMin, tMax);

            // ✅ Y'yi de pencereye göre sıkıştır ki çizgi “doldursun”
            double yMin = y.Min();
            double yMax = y.Max();
            if (Math.Abs(yMax - yMin) < 1e-9)
            {
                yMin -= 0.5;
                yMax += 0.5;
            }
            plt.Axes.SetLimitsY(yMin, yMax);

            _torMiniPlot.Refresh();
        }

        private ScottPlot.Color GetTorSparklineColor(float rawTorr)
        {
            if (rawTorr <= 1e-5f) return ScottPlot.Colors.LimeGreen;
            if (rawTorr <= 1e-3f) return ScottPlot.Colors.DeepSkyBlue;
            if (rawTorr <= 1e-1f) return ScottPlot.Colors.GoldenRod;
            return ScottPlot.Colors.IndianRed;
        }
    }
}
