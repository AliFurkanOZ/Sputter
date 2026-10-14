file_path = r"c:\Users\onure\OneDrive\Desktop\Sputter\SputterNewApp\Models\WorkspaceState.cs"
with open(file_path, "r", encoding="utf-8") as f:
    content = f.read()

# Replace SourceState and StepState definitions
old_block = '''        public SourceState(string name, string target, string life)
        {
            Name = name;
            _target = target;
            _life = life;
        }
    }

    public sealed class StepState : ObservableState
    {
        private decimal _setpoint, _ramp, _dwell;
        private string _remaining = "—";

        public decimal Setpoint { get => _setpoint; set => Set(ref _setpoint, value); }
        public decimal Ramp { get => _ramp; set => Set(ref _ramp, value); }
        public decimal Dwell { get => _dwell; set => Set(ref _dwell, value); }
        public string Remaining { get => _remaining; set => Set(ref _remaining, value); }

        public StepState(decimal setpoint, decimal ramp, decimal dwell)
        {
            _setpoint = setpoint;
            _ramp = ramp;
            _dwell = dwell;
        }
    }'''

new_block = '''        public void UpdateIdlePreviews()
        {
            var dur1 = Services.DepositionSequenceController.ComputeStep1Duration(Step1.Setpoint, Step1.Ramp, Step1.Dwell);
            var dur2 = Services.DepositionSequenceController.ComputeStep2DurationPlanned(Step1.Setpoint, Step2.Setpoint, Step2.Ramp, Step2.Dwell);
            Step1.Remaining = Services.DepositionSequenceController.FormatDurationText(dur1);
            Step2.Remaining = Services.DepositionSequenceController.FormatDurationText(dur2);
        }

        public SourceState(string name, string target, string life)
        {
            Name = name;
            _target = target;
            _life = life;

            Step1.ValueChanged += () =>
            {
                if (IsActive != true) UpdateIdlePreviews();
            };
            Step2.ValueChanged += () =>
            {
                if (IsActive != true) UpdateIdlePreviews();
            };

            UpdateIdlePreviews();
        }
    }

    public sealed class StepState : ObservableState
    {
        private decimal _setpoint, _ramp, _dwell;
        private string _remaining = "—";

        public event Action? ValueChanged;

        public decimal Setpoint
        {
            get => _setpoint;
            set
            {
                if (EqualityComparer<decimal>.Default.Equals(_setpoint, value)) return;
                _setpoint = value;
                Notify(nameof(Setpoint));
                ValueChanged?.Invoke();
            }
        }

        public decimal Ramp
        {
            get => _ramp;
            set
            {
                if (EqualityComparer<decimal>.Default.Equals(_ramp, value)) return;
                _ramp = value;
                Notify(nameof(Ramp));
                ValueChanged?.Invoke();
            }
        }

        public decimal Dwell
        {
            get => _dwell;
            set
            {
                if (EqualityComparer<decimal>.Default.Equals(_dwell, value)) return;
                _dwell = value;
                Notify(nameof(Dwell));
                ValueChanged?.Invoke();
            }
        }

        public string Remaining { get => _remaining; set => Set(ref _remaining, value); }

        public StepState(decimal setpoint, decimal ramp, decimal dwell)
        {
            _setpoint = setpoint;
            _ramp = ramp;
            _dwell = dwell;
        }
    }'''

content_norm = content.replace('\r\n', '\n')
old_norm = old_block.replace('\r\n', '\n')
new_norm = new_block.replace('\r\n', '\n')

assert old_norm in content_norm, "old_norm block not found"
content_norm = content_norm.replace(old_norm, new_norm)

with open(file_path, "w", encoding="utf-8", newline="\n") as f:
    f.write(content_norm)

print("Updated WorkspaceState.cs successfully!")
