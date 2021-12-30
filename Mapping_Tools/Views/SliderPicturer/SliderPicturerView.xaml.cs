using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Drawing;
using Mapping_Tools.Classes;
using Mapping_Tools.Classes.BeatmapHelper;
using Mapping_Tools.Classes.MathUtil;
using Mapping_Tools.Classes.SystemTools;
using Mapping_Tools.Classes.SystemTools.QuickRun;
using Mapping_Tools.Classes.ToolHelpers;
using Mapping_Tools.Classes.Tools;
using Mapping_Tools.Viewmodels;

namespace Mapping_Tools.Views.SliderPicturer {
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    [SmartQuickRunUsage(SmartQuickRunTargets.AnySelection)]
    [VerticalContentScroll]
    [HorizontalContentScroll]
    public partial class SliderPicturerView : IQuickRun, ISavable<SliderPicturerVM> {
        public event EventHandler RunFinished;

        public static readonly string ToolName = "Slider Picturer";

        public static readonly string ToolDescription = $@"Change the length and duration of marked sliders and this tool will automatically handle the SliderVelocity for you.";

        /// <inheritdoc />
        public SliderPicturerView() {
            InitializeComponent();
            Width = MainWindow.AppWindow.content_views.Width;
            Height = MainWindow.AppWindow.content_views.Height;
            DataContext = new SliderPicturerVM();
            ProjectManager.LoadProject(this, message: false);
        }

        public SliderPicturerVM ViewModel => (SliderPicturerVM) DataContext;

        protected override void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
            var bgw = sender as BackgroundWorker;
            e.Result = Picturate((SliderPicturerVM) e.Argument, bgw, e);
        }

       
        private void Start_Click(object sender, RoutedEventArgs e) {
            // Get the current beatmap if the selection mode is 'Selected' because otherwise the selection would always fail
            RunTool(SelectionModeBox.SelectedIndex == 0
                ? new[] {IOHelper.GetCurrentBeatmapOrCurrentBeatmap()}
                : MainWindow.AppWindow.GetCurrentMaps());
        }

        public void QuickRun() {
            RunTool(new[] { IOHelper.GetCurrentBeatmapOrCurrentBeatmap() }, quick: true);
        }

        private void RunTool(string[] paths, bool quick = false) {
            if (!CanRun) return;

            // Remove logical focus to trigger LostFocus on any fields that didn't yet update the ViewModel
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), null);

            BackupManager.SaveMapBackup(paths);

            ViewModel.Paths = paths;
            ViewModel.Quick = quick;

            BackgroundWorker.RunWorkerAsync(ViewModel);
            CanRun = false;
        }

        private string Picturate(SliderPicturerVM arg, BackgroundWorker worker, DoWorkEventArgs _) {
            int slidersCompleted = 0;

            var reader = EditorReaderStuff.GetFullEditorReaderOrNot(out var editorReaderException1);

            if (arg.PictureFile == null) {
                throw new Exception("No image file selected.", editorReaderException1);
            }

            Bitmap img;
            try {
                img = new Bitmap(arg.PictureFile);
            } catch {
                throw new Exception("Not a valid image file.");
            }

            // Recolor bitmap according to slider colors
            // TODO: get actual color values
            // Slider border is always completely opaque (alpha=255)
            Color sliderBorder = Color.FromArgb(255, 255, 255, 255);
            Color sliderColor = Color.FromArgb(0,0,0);
            Color backgroundColor = Color.FromArgb(0,0,0);
            const double LIGHTEN_AMOUNT = 0.5;
            const double DARKEN_AMOUNT = 0.1;
            const byte ALPHA = 100;
            Color innerColor = Color.FromArgb(ALPHA,
                (byte)Math.Min(255, sliderColor.R * (1 + 0.5 * LIGHTEN_AMOUNT) + 255 * LIGHTEN_AMOUNT),
                (byte)Math.Min(255, sliderColor.G * (1 + 0.5 * LIGHTEN_AMOUNT) + 255 * LIGHTEN_AMOUNT),
                (byte)Math.Min(255, sliderColor.B * (1 + 0.5 * LIGHTEN_AMOUNT) + 255 * LIGHTEN_AMOUNT));
            Color outerColor = Color.FromArgb(ALPHA,
                (byte)Math.Min(255, sliderColor.R / (1 + DARKEN_AMOUNT)),
                (byte)Math.Min(255, sliderColor.G / (1 + DARKEN_AMOUNT)),
                (byte)Math.Min(255, sliderColor.B / (1 + DARKEN_AMOUNT)));

            // TODO: 

            // Complete progressbar
            if (worker != null && worker.WorkerReportsProgress)
            {
                worker.ReportProgress(100);
            }

            // Do stuff
            RunFinished?.Invoke(this, new RunToolCompletedEventArgs(true, reader != null, arg.Quick));

            // Make an accurate message
            string message = "";
            if (Math.Abs(slidersCompleted) == 1)
            {
                message += "Successfully completed " + slidersCompleted + " slider!";
            }
            else
            {
                message += "Successfully completed " + slidersCompleted + " sliders!";
            }
            return arg.Quick ? "" : message;
        }
        public SliderPicturerVM GetSaveData() {
            return ViewModel;
        }

        public void SetSaveData(SliderPicturerVM saveData) {
            DataContext = saveData;
        }

        public string AutoSavePath => Path.Combine(MainWindow.AppDataPath, "sliderPicturerproject.json");

        public string DefaultSaveFolder => Path.Combine(MainWindow.AppDataPath, "Slider Picturer Projects");
    }
}
