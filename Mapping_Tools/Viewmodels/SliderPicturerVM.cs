using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Mapping_Tools.Classes.SystemTools;
using Newtonsoft.Json;

namespace Mapping_Tools.Viewmodels
{
    public class SliderPicturerVM : BindableBase
    {
        #region Properties

        [JsonIgnore]
        public string[] Paths { get; set; }

        [JsonIgnore]
        public bool Quick { get; set; }

        private ImportMode _importModeSetting;
        public ImportMode ImportModeSetting
        {
            get => _importModeSetting;
            set
            {
                if (Set(ref _importModeSetting, value)) {
                    RaisePropertyChanged(nameof(TimeCodeBoxVisibility));
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<ImportMode> ImportModes => Enum.GetValues(typeof(ImportMode)).Cast<ImportMode>();

        [JsonIgnore]
        public Visibility TimeCodeBoxVisibility => ImportModeSetting == ImportMode.Time ? Visibility.Visible : Visibility.Collapsed;

        private FreeVariable _freeVariableSetting;
        public FreeVariable FreeVariableSetting
        {
            get => _freeVariableSetting;
            set
            {
                if (Set(ref _freeVariableSetting, value)) {
                    RaisePropertyChanged(nameof(DurationBoxVisibility));
                    RaisePropertyChanged(nameof(EndTimeBoxVisibility));
                    RaisePropertyChanged(nameof(LengthBoxVisibility));
                    RaisePropertyChanged(nameof(VelocityBoxVisibility));
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<FreeVariable> FreeVariables => Enum.GetValues(typeof(FreeVariable)).Cast<FreeVariable>();

        [JsonIgnore]
        public Visibility DurationBoxVisibility => FreeVariableSetting != FreeVariable.Duration && !UseEndTime ? Visibility.Visible : Visibility.Collapsed;

        [JsonIgnore]
        public Visibility EndTimeBoxVisibility => FreeVariableSetting != FreeVariable.Duration && UseEndTime ? Visibility.Visible : Visibility.Collapsed;

        [JsonIgnore]
        public Visibility LengthBoxVisibility => FreeVariableSetting != FreeVariable.Length ? Visibility.Visible : Visibility.Collapsed;

        [JsonIgnore]
        public Visibility VelocityBoxVisibility => FreeVariableSetting != FreeVariable.Velocity ? Visibility.Visible : Visibility.Collapsed;

        private string _timeCode;
        public string TimeCode
        {
            get => _timeCode;
            set => Set(ref _timeCode, value);
        }

        private double _duration;
        public double Duration
        {
            get => _duration;
            set => Set(ref _duration, value);
        }

        private double _endTime;
        public double EndTime
        {
            get => _endTime;
            set => Set(ref _endTime, value);
        }

        private double _length;
        public double Length
        {
            get => _length;
            set => Set(ref _length, value);
        }

        private double _sliderVelocity;
        public double SliderVelocity
        {
            get => _sliderVelocity;
            set => Set(ref _sliderVelocity, value);
        }

        private bool _moveAnchors;
        public bool MoveAnchors
        {
            get => _moveAnchors;
            set => Set(ref _moveAnchors, value);
        }

        private bool _useEndTime;
        public bool UseEndTime
        {
            get => _useEndTime;
            set
            {
                if (Set(ref _useEndTime, value)) {
                    RaisePropertyChanged(nameof(DurationBoxVisibility));
                    RaisePropertyChanged(nameof(EndTimeBoxVisibility));
                }
            }
        }

        private bool _delegateSvToBpm;
        public bool DelegateToBpm
        {
            get => _delegateSvToBpm;
            set => Set(ref _delegateSvToBpm, value);
        }

        private bool _removeSliderTicks;
        public bool RemoveSliderTicks
        {
            get => _removeSliderTicks;
            set => Set(ref _removeSliderTicks, value);
        }

        private ICommand _uploadFile;
        public ICommand UploadFile
        {
            get
            {
                return _uploadFile ?? (_uploadFile = new CommandHandler(() => SetFile(), () => true));
            }
        }

        private String _pictureFile;
        public String PictureFile
        {
            get => _pictureFile;
            set => Set(ref _pictureFile, value);
        }

        private void SetFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".png"; // Required file extension 
            fileDialog.Filter = @"All Image Files|*.BMP;*.bmp;*.JPG;*.JPEG*.jpg;*.jpeg;*.PNG;*.png;*.GIF;*.gif;*.tif;*.tiff;*.ico;*.ICO|PNG|*.PNG;*.png|JPEG|*.JPG;*.JPEG*.jpg;*.jpeg|Bitmap(.BMP,.bmp)|*.BMP;*.bmp|GIF|*.GIF;*.gif|TIF|*.tif;*.tiff|ICO|*.ico;*.ICO";// Optional file extensions

            if (fileDialog.ShowDialog() == DialogResult.OK) {
                PictureFile = fileDialog.FileName;
            }
        }

        #endregion

        public SliderPicturerVM()
        {
            ImportModeSetting = ImportMode.Selected;
            Duration = -1;
            EndTime = -1;
            Length = 1;
            SliderVelocity = -1;
            MoveAnchors = false;
            UseEndTime = false;
            DelegateToBpm = false;
            RemoveSliderTicks = false;
            TimeCode = string.Empty;

        }

        public enum FreeVariable
        {
            Velocity,
            Length,
            Duration
        }

        public enum ImportMode
        {
            Selected,
            Bookmarked,
            Time,
            Everything
        }
    }
    
}