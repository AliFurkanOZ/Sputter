using SputterControlApp.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        private LogService? _logSvc;

        // UI tarafında gösterilecek liste (newest first)
        private readonly BindingSource _evtBs = new();
        private readonly List<LogEvent> _evtAllNewestFirst = new();   // RAM cache (view source)
        private readonly List<LogEvent> _evtView = new();             // filtrelenmiş (grid source)
        private System.Windows.Forms.Timer? _evtSearchTmr;
        private Font? _evtAlarmBoldFont;

        private bool _evtUiReady;

        private enum EvtTimeRange
        {
            All,
            Last1m,
            Last5m,
            Last15m,
            Last1h,
            Today,
            Custom
        }

        private DateTime? _customFrom;
        private DateTime? _customTo;

        // MainForm constructor sonunda çağır
        private void InitEventsUi(LogService logSvc)
        {
            if (_evtUiReady) return;
            _evtUiReady = true;

            _logSvc = logSvc ?? throw new ArgumentNullException(nameof(logSvc));

            // --- ComboBox datasources ---
            cmbEvtLevel.Items.Clear();
            cmbEvtLevel.Items.Add("ALL");
            cmbEvtLevel.Items.AddRange(Enum.GetNames(typeof(LogLevel)));
            cmbEvtLevel.SelectedIndex = 0;

            cmbEvtSource.Items.Clear();
            cmbEvtSource.Items.Add("ALL");
            cmbEvtSource.Items.AddRange(Enum.GetNames(typeof(LogSource)));
            cmbEvtSource.SelectedIndex = 0;

            chkEvtAutoScroll.Checked = true;

            // --- TimeRange ---
            cmbEvtTimeRange.Items.Clear();
            cmbEvtTimeRange.Items.AddRange(new object[]
            {
                "ALL",
                "Last 1m",
                "Last 5m",
                "Last 15m",
                "Last 1h",
                "Today",
                "Custom"
            });
            cmbEvtTimeRange.SelectedItem = "ALL"; 

            // Custom kontrol görünürlüğü (default kapalı)
            SetCustomRangeUiVisible(false);

            // DateTimePicker default değerler
            dtpEvtTo.Value = DateTime.Now;
            dtpEvtFrom.Value = DateTime.Now.AddMinutes(-5);

            // Eventler
            cmbEvtTimeRange.SelectedIndexChanged += (_, __) =>
            {
                var mode = ParseTimeRange(cmbEvtTimeRange.SelectedItem?.ToString());
                bool custom = mode == EvtTimeRange.Custom;
                SetCustomRangeUiVisible(custom);

                if (!custom)
                {
                    // preset seçildiyse custom aralığı temizle
                    _customFrom = null;
                    _customTo = null;
                    ApplyEventsFilter();
                }
            };

            btnEvtApplyRange.Click += (_, __) =>
            {
                _customFrom = dtpEvtFrom.Value;
                _customTo = dtpEvtTo.Value;

                // güvenlik: from > to ise swap
                if (_customFrom > _customTo)
                {
                    var tmp = _customFrom;
                    _customFrom = _customTo;
                    _customTo = tmp;
                }

                ApplyEventsFilter();
            };

            // --- Grid init ---
            InitEventsGrid();

            // --- Binding ---
            _evtBs.DataSource = _evtView;
            dgvEvents.DataSource = _evtBs;

            // --- Events ---
            cmbEvtLevel.SelectedIndexChanged += (_, __) => ApplyEventsFilter();
            cmbEvtSource.SelectedIndexChanged += (_, __) => ApplyEventsFilter();
            btnEvtExport.Click += (_, __) => ExportEvents();
            button4.Click += (_, __) => ClearEvents();
            // --- Search debounce ---
            _evtSearchTmr = new System.Windows.Forms.Timer { Interval = 500 };
            _evtSearchTmr.Tick += (_, __) =>
            {
                _evtSearchTmr!.Stop();
                ApplyEventsFilter();
            };

            txtEvtSearch.TextChanged += (_, __) =>
            {
                _evtSearchTmr!.Stop();
                _evtSearchTmr.Start();
            };


            dgvEvents.CellFormatting += DgvEvents_CellFormatting;

            // --- Load initial snapshot ---
            var snap = _logSvc.SnapshotNewestFirst();
            _evtAllNewestFirst.Clear();
            _evtAllNewestFirst.AddRange(snap);
            ApplyEventsFilter();

            // --- Live subscribe ---
            _logSvc.OnNewEvent += LogSvc_OnNewEvent;

            UpdateEvtStats();
        }

        private void InitEventsGrid()
        {
            dgvEvents.AutoGenerateColumns = false;
            dgvEvents.ReadOnly = true;
            dgvEvents.AllowUserToAddRows = false;
            dgvEvents.AllowUserToDeleteRows = false;
            dgvEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEvents.MultiSelect = false;
            dgvEvents.RowHeadersVisible = false;
            dgvEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEvents.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            _evtAlarmBoldFont ??= new Font(dgvEvents.Font, FontStyle.Bold);

            dgvEvents.Columns.Clear();

            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTime",
                HeaderText = "Time",
                DataPropertyName = nameof(LogEvent.TimeText),
                FillWeight = 20
            });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colType",
                HeaderText = "Type",
                DataPropertyName = nameof(LogEvent.LevelText),
                FillWeight = 10
            });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCode",
                HeaderText = "Code",
                DataPropertyName = nameof(LogEvent.Code),
                FillWeight = 10
            });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMessage",
                HeaderText = "Message",
                DataPropertyName = nameof(LogEvent.Message),
                FillWeight = 45
            });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSource",
                HeaderText = "Source",
                DataPropertyName = nameof(LogEvent.Source),
                FillWeight = 15
            });
            dgvEvents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRegistrationNo",
                HeaderText = "Registration No",
                DataPropertyName = nameof(LogEvent.RegistrationNo),
                FillWeight = 15
            });
        }

        private void LogSvc_OnNewEvent(LogEvent ev)
        {
            // LogService thread'i UI olmayabilir -> UI helper ile
            UI(() =>
            {
                // Kullanıcı üstteyse (newest visible) auto-scroll davranışı
                bool userAtTop = true;
                if (dgvEvents.RowCount > 0)
                {
                    try
                    {
                        userAtTop = dgvEvents.FirstDisplayedScrollingRowIndex == 0;
                    }
                    catch { userAtTop = true; }
                }

                // newest first: başa ekle
                _evtAllNewestFirst.Insert(0, ev);

                // RAM kırpma (UI cache de limitli olsun)
                if (_logSvc != null && _evtAllNewestFirst.Count > _logSvc.Capacity)
                    _evtAllNewestFirst.RemoveAt(_evtAllNewestFirst.Count - 1);

                ApplyEventsFilter();

                if (chkEvtAutoScroll.Checked && userAtTop)
                {
                    // newest first -> en üste taşı
                    if (dgvEvents.RowCount > 0)
                    {
                        try
                        {
                            dgvEvents.FirstDisplayedScrollingRowIndex = 0;
                            dgvEvents.ClearSelection();
                        }
                        catch { }
                    }
                }

                UpdateEvtStats();
            });
        }

        private void ApplyEventsFilter()
        {
            // Filtre: Level / Source / Search
            string levelSel = (cmbEvtLevel.SelectedItem?.ToString() ?? "ALL").Trim();
            string srcSel = (cmbEvtSource.SelectedItem?.ToString() ?? "ALL").Trim();
            string q = (txtEvtSearch.Text ?? "").Trim();

            bool levelAll = levelSel.Equals("ALL", StringComparison.OrdinalIgnoreCase);
            bool srcAll = srcSel.Equals("ALL", StringComparison.OrdinalIgnoreCase);

            IEnumerable<LogEvent> query = _evtAllNewestFirst;


            if (!levelAll && Enum.TryParse<LogLevel>(levelSel, out var lv))
                query = query.Where(x => x.Level == lv);

            if (!srcAll && Enum.TryParse<LogSource>(srcSel, out var src))
                query = query.Where(x => x.Source == src);

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(x =>
                    (x.Message?.IndexOf(q, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                    (x.Code?.IndexOf(q, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                    (x.RegistrationNo?.IndexOf(q, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0);
            }

            // ---- TimeRange ----
            var mode = ParseTimeRange(cmbEvtTimeRange.SelectedItem?.ToString());
            if (mode == EvtTimeRange.Custom)
            {
                if (_customFrom.HasValue)
                    query = query.Where(x => x.Time >= _customFrom.Value);

                if (_customTo.HasValue)
                    query = query.Where(x => x.Time <= _customTo.Value);
            }
            else
            {
                var cutoff = GetTimeCutoffUtcOrLocal();
                if (cutoff.HasValue)
                    query = query.Where(x => x.Time >= cutoff.Value);
            }
            // ---- TimeRange sonu ----

            _evtView.Clear();
            _evtView.AddRange(query);

            _evtBs.ResetBindings(false);
            UpdateEvtStats();
        }

        private void UpdateEvtStats()
        {
            if (lblEvtStats == null) return;
            lblEvtStats.Text = $"Shown: {_evtView.Count} / Total: {_evtAllNewestFirst.Count}";
        }

        private void DgvEvents_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvEvents.RowCount) return;
            if (dgvEvents.Rows[e.RowIndex].DataBoundItem is not LogEvent ev) return;

            Color back;
            Color fore;

            switch (ev.Level)
            {
                case LogLevel.Info:
                    back = Color.White;
                    fore = Color.Black;
                    break;
                case LogLevel.Warn:
                    back = Color.Goldenrod;
                    fore = Color.Black;
                    break;
                case LogLevel.Error:
                    back = Color.IndianRed;
                    fore = Color.White;
                    break;
                case LogLevel.Alarm:
                    back = Color.Red;
                    fore = Color.White;
                    if (_evtAlarmBoldFont != null)
                        dgvEvents.Rows[e.RowIndex].DefaultCellStyle.Font = _evtAlarmBoldFont;
                    break;
                default:
                    back = Color.White;
                    fore = Color.Black;
                    break;
            }

            if (ev.Level != LogLevel.Alarm)
                dgvEvents.Rows[e.RowIndex].DefaultCellStyle.Font = dgvEvents.Font;

            dgvEvents.Rows[e.RowIndex].DefaultCellStyle.BackColor = back;
            dgvEvents.Rows[e.RowIndex].DefaultCellStyle.ForeColor = fore;
        }

        private static EvtTimeRange ParseTimeRange(string? s)
        {
            s = (s ?? "ALL").Trim().ToUpperInvariant();
            return s switch
            {
                "LAST 1M" => EvtTimeRange.Last1m,
                "LAST 5M" => EvtTimeRange.Last5m,
                "LAST 15M" => EvtTimeRange.Last15m,
                "LAST 1H" => EvtTimeRange.Last1h,
                "TODAY" => EvtTimeRange.Today,
                "CUSTOM" => EvtTimeRange.Custom,
                _ => EvtTimeRange.All
            };
        }

        private void SetCustomRangeUiVisible(bool visible)
        {
            if (dtpEvtFrom != null) dtpEvtFrom.Visible = visible;
            if (dtpEvtTo != null) dtpEvtTo.Visible = visible;
            if (btnEvtApplyRange != null) btnEvtApplyRange.Visible = visible;
        }

        private DateTime? GetTimeCutoffUtcOrLocal()
        {
            // Biz Time'ı local tutuyoruz, cutoff da local hesaplanacak.
            var mode = ParseTimeRange(cmbEvtTimeRange.SelectedItem?.ToString());
            DateTime now = DateTime.Now;

            return mode switch
            {
                EvtTimeRange.Last1m => now.AddMinutes(-1),
                EvtTimeRange.Last5m => now.AddMinutes(-5),
                EvtTimeRange.Last15m => now.AddMinutes(-15),
                EvtTimeRange.Last1h => now.AddHours(-1),
                EvtTimeRange.Today => now.Date,
                _ => null
            };
        }
        private void TeardownEventsUi()
        {
            if (_logSvc != null)
                _logSvc.OnNewEvent -= LogSvc_OnNewEvent;

            if (_evtSearchTmr != null)
            {
                _evtSearchTmr.Stop();
                _evtSearchTmr.Dispose();
                _evtSearchTmr = null;
            }

            if (_evtAlarmBoldFont != null)
            {
                _evtAlarmBoldFont.Dispose();
                _evtAlarmBoldFont = null;
            }
        }

        private void ClearEvents()
        {
            if (_logSvc == null)
                return;

            var answer = MessageBox.Show(
                "Tüm event log kayıtları silinsin mi?",
                "Clear Log",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
                return;

            _logSvc.Clear();
            _evtAllNewestFirst.Clear();
            _evtView.Clear();
            _evtBs.ResetBindings(false);
            UpdateEvtStats();
            PostStatus("Event log kayıtları temizlendi.");
        }

        private void ExportEvents()
        {
            try
            {
                using var sfd = new SaveFileDialog
                {
                    Title = "Export Events",
                    Filter = "CSV (*.csv)|*.csv|Text (*.txt)|*.txt",
                    FileName = $"events_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    OverwritePrompt = true
                };

                if (sfd.ShowDialog(this) != DialogResult.OK)
                    return;

                var path = sfd.FileName;
                bool isCsv = path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);

                // Şu an ekranda görünen (filtrelenmiş) listeyi export edelim
                var list = _evtView.ToList();

                if (isCsv)
                    System.IO.File.WriteAllText(path, BuildCsv(list), System.Text.Encoding.UTF8);
                else
                    System.IO.File.WriteAllText(path, BuildTxt(list), System.Text.Encoding.UTF8);

                PostStatus($"Exported: {list.Count} rows → {path}");
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
                MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string BuildCsv(List<Logging.LogEvent> rows)
        {
            // Basit CSV escape (quote, comma, newline)
            static string Esc(string? s)
            {
                s ??= "";
                bool mustQuote = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
                if (s.Contains('"')) s = s.Replace("\"", "\"\"");
                return mustQuote ? $"\"{s}\"" : s;
            }

            var sb = new System.Text.StringBuilder(rows.Count * 80);
            sb.AppendLine("Time,Type,Code,Message,Source,RegistrationNo");

            foreach (var r in rows)
            {
                sb.Append(Esc(r.TimeText)).Append(',')
                  .Append(Esc(r.LevelText)).Append(',')
                  .Append(Esc(r.Code)).Append(',')
                  .Append(Esc(r.Message)).Append(',')
                  .Append(Esc(r.Source.ToString())).Append(',')
                  .Append(Esc(r.RegistrationNo))
                  .AppendLine();
            }
            return sb.ToString();
        }

        private static string BuildTxt(List<Logging.LogEvent> rows)
        {
            var sb = new System.Text.StringBuilder(rows.Count * 80);
            foreach (var r in rows)
            {
                sb.Append('[').Append(r.TimeText).Append("] ")
                  .Append(r.LevelText).Append(' ')
                  .Append(r.Source).Append(' ')
                  .Append('[').Append(r.RegistrationNo).Append("] ")
                  .Append(r.Code).Append(" - ")
                  .AppendLine(r.Message);
            }
            return sb.ToString();
        }


        // Test için: hızlı dummy bas
        private void Events_TestDump()
        {
            if (_logSvc == null) return;

            _logSvc.Publish(new LogEvent { Level = LogLevel.Info, Code = "0", Source = LogSource.System, Message = "App started." });
            _logSvc.Publish(new LogEvent { Level = LogLevel.Warn, Code = "W12", Source = LogSource.VacuumControl, Message = "TMP vent valve opened." });
            _logSvc.Publish(new LogEvent { Level = LogLevel.Error, Code = "E07", Source = LogSource.PLC, Message = "Communication timeout." });
            _logSvc.Publish(new LogEvent { Level = LogLevel.Alarm, Code = "A01", Source = LogSource.SampleControl, Message = "Interlock triggered!" });
        }
    }
}
