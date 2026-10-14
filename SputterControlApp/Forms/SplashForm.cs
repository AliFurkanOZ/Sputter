using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SputterControlApp
{
    public sealed class SplashForm : Form
    {
        private const int ContentVerticalOffset = 42;
        private readonly Panel _contentHost = new Panel();
        private readonly Panel _spinnerHost = new Panel();
        private readonly LoadingSpinner _spinner = new LoadingSpinner();
        private readonly Label _titleLabel = new Label();
        private readonly Label _subtitleLabel = new Label();

        public SplashForm()
        {
            SuspendLayout();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint,
                true);
            UpdateStyles();
            DoubleBuffered = true;

            AutoScaleMode = AutoScaleMode.None;
            var splashImage = Properties.Resources.Splash_old;
            BackgroundImage = splashImage;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = CalculateClientSize(splashImage.Size);
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = nameof(SplashForm);
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;

            _contentHost.Name = "pnlContentHost";
            _contentHost.Size = new Size(360, 270);
            _contentHost.Anchor = AnchorStyles.None;
            _contentHost.BackColor = Color.Transparent;

            _spinnerHost.Name = "pnlSpinnerHost";
            _spinnerHost.Size = new Size(160, 160);
            _spinnerHost.Location = new Point(100, 10);
            _spinnerHost.BackColor = Color.Transparent;
            _spinnerHost.Padding = new Padding(24);

            _spinner.Dock = DockStyle.Fill;
            _spinnerHost.Controls.Add(_spinner);

            _titleLabel.Name = "lblSplashTitle";
            _titleLabel.Size = new Size(_contentHost.Width, 34);
            _titleLabel.Location = new Point(0, 178);
            _titleLabel.BackColor = Color.Transparent;
            _titleLabel.ForeColor = Color.FromArgb(19, 53, 84);
            _titleLabel.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            _titleLabel.Text = "Loading...";
            _titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            _subtitleLabel.Name = "lblSplashSubtitle";
            _subtitleLabel.Size = new Size(_contentHost.Width, 24);
            _subtitleLabel.Location = new Point(0, 214);
            _subtitleLabel.BackColor = Color.Transparent;
            _subtitleLabel.ForeColor = Color.FromArgb(58, 86, 110);
            _subtitleLabel.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            _subtitleLabel.Text = "Reading all device data...";
            _subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;

            _contentHost.Controls.Add(_spinnerHost);
            _contentHost.Controls.Add(_titleLabel);
            _contentHost.Controls.Add(_subtitleLabel);

            Controls.Add(_contentHost);

            Load += (_, __) => CenterContent();
            Resize += (_, __) => CenterContent();

            ResumeLayout(false);
        }

        public Panel SpinnerHost => _spinnerHost;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _spinner.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _spinner.Stop();
            base.OnFormClosed(e);
        }

        private static Size CalculateClientSize(Size imageSize)
        {
            Rectangle workArea = Screen.PrimaryScreen?.WorkingArea
                ?? new Rectangle(0, 0, imageSize.Width, imageSize.Height);

            int maxWidth = Math.Max(700, (int)(workArea.Width * 0.50));
            int maxHeight = Math.Max(480, (int)(workArea.Height * 0.62));
            double scale = Math.Min(
                Math.Min((double)maxWidth / imageSize.Width, (double)maxHeight / imageSize.Height),
                1d);

            return new Size(
                Math.Max(700, (int)Math.Round(imageSize.Width * scale)),
                Math.Max(460, (int)Math.Round(imageSize.Height * scale)));
        }

        private void CenterContent()
        {
            _contentHost.Left = Math.Max(0, (ClientSize.Width - _contentHost.Width) / 2);
            _contentHost.Top = Math.Max(0, ((ClientSize.Height - _contentHost.Height) / 2) + ContentVerticalOffset);
        }

        private sealed class LoadingSpinner : Control
        {
            private const int DotCount = 12;
            private static readonly Color AccentColor = Color.FromArgb(55, 145, 189);
            private static readonly Color AccentHighlightColor = Color.FromArgb(170, 233, 255);
            private static readonly Color AccentShadowColor = Color.FromArgb(18, 53, 76);
            private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
            private int _activeIndex;

            public LoadingSpinner()
            {
                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint,
                    true);

                BackColor = Color.Transparent;
                DoubleBuffered = true;
                Margin = Padding.Empty;
                MinimumSize = new Size(72, 72);

                _timer.Interval = 80;
                _timer.Tick += (_, __) =>
                {
                    _activeIndex = (_activeIndex + 1) % DotCount;
                    Invalidate();
                };
            }

            public void Start()
            {
                if (!_timer.Enabled)
                    _timer.Start();
            }

            public void Stop()
            {
                if (_timer.Enabled)
                    _timer.Stop();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _timer.Stop();
                    _timer.Dispose();
                }

                base.Dispose(disposing);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                float size = Math.Min(Width, Height);
                float centerX = Width / 2f;
                float centerY = Height / 2f;
                float orbitRadius = size * 0.28f;
                float dotSize = Math.Max(6f, size * 0.11f);
                float backgroundSize = size * 0.82f;

                using (var backgroundBrush = new SolidBrush(Color.FromArgb(90, AccentShadowColor)))
                {
                    e.Graphics.FillEllipse(
                        backgroundBrush,
                        centerX - (backgroundSize / 2f),
                        centerY - (backgroundSize / 2f),
                        backgroundSize,
                        backgroundSize);
                }

                using (var ringPen = new Pen(Color.FromArgb(90, AccentColor), Math.Max(2f, size * 0.02f)))
                {
                    float ringInset = Math.Max(6f, size * 0.08f);
                    e.Graphics.DrawEllipse(
                        ringPen,
                        centerX - (backgroundSize / 2f) + ringInset,
                        centerY - (backgroundSize / 2f) + ringInset,
                        backgroundSize - (ringInset * 2f),
                        backgroundSize - (ringInset * 2f));
                }

                for (int i = 0; i < DotCount; i++)
                {
                    int distanceFromActive = (i - _activeIndex + DotCount) % DotCount;
                    float intensity = 1f - (distanceFromActive / (float)DotCount);
                    int alpha = 45 + (int)(210 * intensity);
                    Color dotColor = BlendColor(AccentColor, AccentHighlightColor, intensity);

                    double angle = ((Math.PI * 2d) / DotCount * i) - (Math.PI / 2d);
                    float x = centerX + (float)Math.Cos(angle) * orbitRadius - (dotSize / 2f);
                    float y = centerY + (float)Math.Sin(angle) * orbitRadius - (dotSize / 2f);

                    using var dotBrush = new SolidBrush(Color.FromArgb(alpha, dotColor));
                    e.Graphics.FillEllipse(dotBrush, x, y, dotSize, dotSize);
                }
            }

            private static Color BlendColor(Color from, Color to, float amount)
            {
                amount = Math.Max(0f, Math.Min(1f, amount));

                int r = from.R + (int)((to.R - from.R) * amount);
                int g = from.G + (int)((to.G - from.G) * amount);
                int b = from.B + (int)((to.B - from.B) * amount);

                return Color.FromArgb(r, g, b);
            }
        }
    }
}
