using NationalInstruments.DAQmx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PhotdiodeInterface
{
    public class MainForm : System.Windows.Forms.Form
    {
        private Task myTask;
        private Task runningTask;
        private AnalogMultiChannelReader analogInReader;
        private AsyncCallback analogCallback;
        private ArrayList savedData;
        private StreamWriter timestampWriter;
        private StreamWriter fileStreamWriter;
        private BinaryWriter fileBinaryWriter;
        private ToolTip fileToolTip;
        private GroupBox timingParametersGroupBox;
        private GroupBox channelParametersGroupBox;
        private GroupBox writeToFileGroupBox;
        private GroupBox triggerParametersGroupBox;
        private GroupBox plotParametersBox;
        private GroupBox plotBox;
        private CheckBox saveStatisticsCheckBox;
        private GroupBox statisticsBox;
        private Panel saveFileTypePanel;
        private Panel pauseTriggerPanel;
        private Label maximumLabel;
        private Label minimumLabel;
        private Label physicalChannelLabel;
        private Label rateLabel;
        private Label fileTypeWriteLabel;
        private Label triggerSourceLabel;
        private Label horizontalScaleLabel;
        private Label verticalScaleLabel;
        private Label filePathWriteLabel;
        private Label pauseTriggerLabel;
        private Label statisticsFrequencyLabel;
        private TextBox filePathWriteTextBox;
        private TextBox triggerSourceTextBox;
        private NumericUpDown rateNumeric;
        private NumericUpDown minimumValueNumeric;
        private NumericUpDown maximumValueNumeric;
        private NumericUpDown horizontalScaleNumeric;
        private NumericUpDown verticalScaleNumeric;
        private NumericUpDown statisticsFrequencyNumeric;
        private CheckBox plottingCheckBox;
        private CheckBox autoScaleCheckBox;
        private ComboBox physicalChannelComboBox;
        private FolderBrowserDialog outputFolderDialog;
        private Button browseWriteButton;
        private Button stopButton;
        private Button startButton;
        private RadioButton binaryFileWriteRadioButton;
        private RadioButton textFileWriteRadioButton;
        private RadioButton pauseWhenHighButton;
        private RadioButton pauseWhenLowButton;
        private Chart waveformChart;
        private Queue<double> plotHistory = new Queue<double>();
        private DigitalLevelPauseTriggerCondition gateLevel = DigitalLevelPauseTriggerCondition.High;
        private IContainer components;
        private DateTime lastPlotUpdate = DateTime.MinValue;
        private DateTime acquisitionStart;
        private bool useTextFileWrite;
        private bool recordingEnabled = false;
        private bool plottingEnabled = false;
        private bool statisticsEnabled = true;
        private long totalSamplesAcquired = 0;
        private long statisticsSamplesCollected;
        private double[,] data;
        private double samplesPerStatisticsInterval;
        private double nextStatisticsSample;
        private double statisticsSum;
        private double statisticsSumSq;
        private double statisticsMin;
        private double statisticsMax;
        private string outputRootFolder;
        private string sessionFolder;
        
        public MainForm()
        {
            InitializeComponent();

            physicalChannelComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External));
            if (physicalChannelComboBox.Items.Count > 0)
                physicalChannelComboBox.SelectedIndex = 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (myTask != null)
                {
                    runningTask = null;
                    myTask.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.channelParametersGroupBox = new System.Windows.Forms.GroupBox();
            this.physicalChannelComboBox = new System.Windows.Forms.ComboBox();
            this.minimumValueNumeric = new System.Windows.Forms.NumericUpDown();
            this.maximumValueNumeric = new System.Windows.Forms.NumericUpDown();
            this.maximumLabel = new System.Windows.Forms.Label();
            this.minimumLabel = new System.Windows.Forms.Label();
            this.physicalChannelLabel = new System.Windows.Forms.Label();
            this.timingParametersGroupBox = new System.Windows.Forms.GroupBox();
            this.rateNumeric = new System.Windows.Forms.NumericUpDown();
            this.rateLabel = new System.Windows.Forms.Label();
            this.filePathWriteTextBox = new System.Windows.Forms.TextBox();
            this.fileToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.writeToFileGroupBox = new System.Windows.Forms.GroupBox();
            this.saveFileTypePanel = new System.Windows.Forms.Panel();
            this.textFileWriteRadioButton = new System.Windows.Forms.RadioButton();
            this.binaryFileWriteRadioButton = new System.Windows.Forms.RadioButton();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.browseWriteButton = new System.Windows.Forms.Button();
            this.filePathWriteLabel = new System.Windows.Forms.Label();
            this.fileTypeWriteLabel = new System.Windows.Forms.Label();
            this.outputFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveStatisticsCheckBox = new System.Windows.Forms.CheckBox();
            this.triggerParametersGroupBox = new System.Windows.Forms.GroupBox();
            this.pauseTriggerPanel = new System.Windows.Forms.Panel();
            this.pauseWhenLowButton = new System.Windows.Forms.RadioButton();
            this.pauseWhenHighButton = new System.Windows.Forms.RadioButton();
            this.pauseTriggerLabel = new System.Windows.Forms.Label();
            this.triggerSourceLabel = new System.Windows.Forms.Label();
            this.triggerSourceTextBox = new System.Windows.Forms.TextBox();
            this.waveformChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.horizontalScaleNumeric = new System.Windows.Forms.NumericUpDown();
            this.plotParametersBox = new System.Windows.Forms.GroupBox();
            this.autoScaleCheckBox = new System.Windows.Forms.CheckBox();
            this.verticalScaleNumeric = new System.Windows.Forms.NumericUpDown();
            this.verticalScaleLabel = new System.Windows.Forms.Label();
            this.plottingCheckBox = new System.Windows.Forms.CheckBox();
            this.horizontalScaleLabel = new System.Windows.Forms.Label();
            this.plotBox = new System.Windows.Forms.GroupBox();
            this.statisticsBox = new System.Windows.Forms.GroupBox();
            this.statisticsFrequencyNumeric = new System.Windows.Forms.NumericUpDown();
            this.statisticsFrequencyLabel = new System.Windows.Forms.Label();
            this.channelParametersGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minimumValueNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maximumValueNumeric)).BeginInit();
            this.timingParametersGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rateNumeric)).BeginInit();
            this.writeToFileGroupBox.SuspendLayout();
            this.saveFileTypePanel.SuspendLayout();
            this.triggerParametersGroupBox.SuspendLayout();
            this.pauseTriggerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waveformChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalScaleNumeric)).BeginInit();
            this.plotParametersBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.verticalScaleNumeric)).BeginInit();
            this.plotBox.SuspendLayout();
            this.statisticsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statisticsFrequencyNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // channelParametersGroupBox
            // 
            this.channelParametersGroupBox.Controls.Add(this.physicalChannelComboBox);
            this.channelParametersGroupBox.Controls.Add(this.minimumValueNumeric);
            this.channelParametersGroupBox.Controls.Add(this.maximumValueNumeric);
            this.channelParametersGroupBox.Controls.Add(this.maximumLabel);
            this.channelParametersGroupBox.Controls.Add(this.minimumLabel);
            this.channelParametersGroupBox.Controls.Add(this.physicalChannelLabel);
            this.channelParametersGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.channelParametersGroupBox.Location = new System.Drawing.Point(8, 8);
            this.channelParametersGroupBox.Name = "channelParametersGroupBox";
            this.channelParametersGroupBox.Size = new System.Drawing.Size(224, 112);
            this.channelParametersGroupBox.TabIndex = 0;
            this.channelParametersGroupBox.TabStop = false;
            this.channelParametersGroupBox.Text = "Channel Parameters";
            // 
            // physicalChannelComboBox
            // 
            this.physicalChannelComboBox.Location = new System.Drawing.Point(126, 23);
            this.physicalChannelComboBox.Name = "physicalChannelComboBox";
            this.physicalChannelComboBox.Size = new System.Drawing.Size(90, 21);
            this.physicalChannelComboBox.TabIndex = 1;
            this.physicalChannelComboBox.Text = "Dev1/ai0";
            // 
            // minimumValueNumeric
            // 
            this.minimumValueNumeric.DecimalPlaces = 2;
            this.minimumValueNumeric.Location = new System.Drawing.Point(126, 53);
            this.minimumValueNumeric.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.minimumValueNumeric.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.minimumValueNumeric.Name = "minimumValueNumeric";
            this.minimumValueNumeric.Size = new System.Drawing.Size(90, 20);
            this.minimumValueNumeric.TabIndex = 3;
            this.minimumValueNumeric.ValueChanged += new System.EventHandler(this.verticalScaleNumeric_ValueChanged);
            // 
            // maximumValueNumeric
            // 
            this.maximumValueNumeric.DecimalPlaces = 2;
            this.maximumValueNumeric.Location = new System.Drawing.Point(126, 83);
            this.maximumValueNumeric.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.maximumValueNumeric.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.maximumValueNumeric.Name = "maximumValueNumeric";
            this.maximumValueNumeric.Size = new System.Drawing.Size(90, 20);
            this.maximumValueNumeric.TabIndex = 5;
            this.maximumValueNumeric.Value = new decimal(new int[] {
            100,
            0,
            0,
            65536});
            this.maximumValueNumeric.ValueChanged += new System.EventHandler(this.verticalScaleNumeric_ValueChanged);
            // 
            // maximumLabel
            // 
            this.maximumLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.maximumLabel.Location = new System.Drawing.Point(16, 85);
            this.maximumLabel.Name = "maximumLabel";
            this.maximumLabel.Size = new System.Drawing.Size(112, 16);
            this.maximumLabel.TabIndex = 4;
            this.maximumLabel.Text = "Maximum Value (V):";
            // 
            // minimumLabel
            // 
            this.minimumLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.minimumLabel.Location = new System.Drawing.Point(16, 55);
            this.minimumLabel.Name = "minimumLabel";
            this.minimumLabel.Size = new System.Drawing.Size(104, 15);
            this.minimumLabel.TabIndex = 2;
            this.minimumLabel.Text = "Minimum Value (V):";
            // 
            // physicalChannelLabel
            // 
            this.physicalChannelLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.physicalChannelLabel.Location = new System.Drawing.Point(16, 25);
            this.physicalChannelLabel.Name = "physicalChannelLabel";
            this.physicalChannelLabel.Size = new System.Drawing.Size(96, 16);
            this.physicalChannelLabel.TabIndex = 0;
            this.physicalChannelLabel.Text = "Physical Channel:";
            // 
            // timingParametersGroupBox
            // 
            this.timingParametersGroupBox.Controls.Add(this.rateNumeric);
            this.timingParametersGroupBox.Controls.Add(this.rateLabel);
            this.timingParametersGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.timingParametersGroupBox.Location = new System.Drawing.Point(8, 126);
            this.timingParametersGroupBox.Name = "timingParametersGroupBox";
            this.timingParametersGroupBox.Size = new System.Drawing.Size(224, 50);
            this.timingParametersGroupBox.TabIndex = 1;
            this.timingParametersGroupBox.TabStop = false;
            this.timingParametersGroupBox.Text = "Timing Parameters";
            // 
            // rateNumeric
            // 
            this.rateNumeric.DecimalPlaces = 2;
            this.rateNumeric.Location = new System.Drawing.Point(126, 23);
            this.rateNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.rateNumeric.Name = "rateNumeric";
            this.rateNumeric.Size = new System.Drawing.Size(90, 20);
            this.rateNumeric.TabIndex = 3;
            this.rateNumeric.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.rateNumeric.ValueChanged += new System.EventHandler(this.rateNumeric_ValueChanged);
            // 
            // rateLabel
            // 
            this.rateLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rateLabel.Location = new System.Drawing.Point(16, 25);
            this.rateLabel.Name = "rateLabel";
            this.rateLabel.Size = new System.Drawing.Size(56, 16);
            this.rateLabel.TabIndex = 2;
            this.rateLabel.Text = "Rate (Hz):";
            // 
            // filePathWriteTextBox
            // 
            this.filePathWriteTextBox.Location = new System.Drawing.Point(120, 57);
            this.filePathWriteTextBox.Name = "filePathWriteTextBox";
            this.filePathWriteTextBox.ReadOnly = true;
            this.filePathWriteTextBox.Size = new System.Drawing.Size(585, 20);
            this.filePathWriteTextBox.TabIndex = 4;
            this.filePathWriteTextBox.Text = "Choose file location";
            // 
            // writeToFileGroupBox
            // 
            this.writeToFileGroupBox.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.writeToFileGroupBox.Controls.Add(this.saveFileTypePanel);
            this.writeToFileGroupBox.Controls.Add(this.stopButton);
            this.writeToFileGroupBox.Controls.Add(this.startButton);
            this.writeToFileGroupBox.Controls.Add(this.browseWriteButton);
            this.writeToFileGroupBox.Controls.Add(this.filePathWriteLabel);
            this.writeToFileGroupBox.Controls.Add(this.filePathWriteTextBox);
            this.writeToFileGroupBox.Controls.Add(this.fileTypeWriteLabel);
            this.writeToFileGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.writeToFileGroupBox.Location = new System.Drawing.Point(238, 343);
            this.writeToFileGroupBox.Name = "writeToFileGroupBox";
            this.writeToFileGroupBox.Size = new System.Drawing.Size(741, 120);
            this.writeToFileGroupBox.TabIndex = 2;
            this.writeToFileGroupBox.TabStop = false;
            this.writeToFileGroupBox.Text = "Write To File";
            // 
            // saveFileTypePanel
            // 
            this.saveFileTypePanel.Controls.Add(this.textFileWriteRadioButton);
            this.saveFileTypePanel.Controls.Add(this.binaryFileWriteRadioButton);
            this.saveFileTypePanel.Location = new System.Drawing.Point(118, 19);
            this.saveFileTypePanel.Name = "saveFileTypePanel";
            this.saveFileTypePanel.Size = new System.Drawing.Size(178, 28);
            this.saveFileTypePanel.TabIndex = 8;
            // 
            // textFileWriteRadioButton
            // 
            this.textFileWriteRadioButton.Checked = true;
            this.textFileWriteRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.textFileWriteRadioButton.Location = new System.Drawing.Point(3, 6);
            this.textFileWriteRadioButton.Name = "textFileWriteRadioButton";
            this.textFileWriteRadioButton.Size = new System.Drawing.Size(72, 16);
            this.textFileWriteRadioButton.TabIndex = 1;
            this.textFileWriteRadioButton.TabStop = true;
            this.textFileWriteRadioButton.Text = "Text File";
            this.textFileWriteRadioButton.CheckedChanged += new System.EventHandler(this.fileWriteRadioButton_CheckedChanged);
            // 
            // binaryFileWriteRadioButton
            // 
            this.binaryFileWriteRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.binaryFileWriteRadioButton.Location = new System.Drawing.Point(81, 6);
            this.binaryFileWriteRadioButton.Name = "binaryFileWriteRadioButton";
            this.binaryFileWriteRadioButton.Size = new System.Drawing.Size(72, 16);
            this.binaryFileWriteRadioButton.TabIndex = 2;
            this.binaryFileWriteRadioButton.Text = "Binary File";
            this.binaryFileWriteRadioButton.CheckedChanged += new System.EventHandler(this.fileWriteRadioButton_CheckedChanged);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.stopButton.Location = new System.Drawing.Point(216, 88);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(80, 24);
            this.stopButton.TabIndex = 7;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.startButton.Location = new System.Drawing.Point(120, 88);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(80, 24);
            this.startButton.TabIndex = 6;
            this.startButton.Text = "Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // browseWriteButton
            // 
            this.browseWriteButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseWriteButton.Location = new System.Drawing.Point(711, 55);
            this.browseWriteButton.Name = "browseWriteButton";
            this.browseWriteButton.Size = new System.Drawing.Size(24, 23);
            this.browseWriteButton.TabIndex = 5;
            this.browseWriteButton.Text = "...";
            this.browseWriteButton.Click += new System.EventHandler(this.browseWriteButton_Click);
            // 
            // filePathWriteLabel
            // 
            this.filePathWriteLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.filePathWriteLabel.Location = new System.Drawing.Point(16, 59);
            this.filePathWriteLabel.Name = "filePathWriteLabel";
            this.filePathWriteLabel.Size = new System.Drawing.Size(72, 16);
            this.filePathWriteLabel.TabIndex = 3;
            this.filePathWriteLabel.Text = "Output Folder:";
            // 
            // fileTypeWriteLabel
            // 
            this.fileTypeWriteLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.fileTypeWriteLabel.Location = new System.Drawing.Point(16, 24);
            this.fileTypeWriteLabel.Name = "fileTypeWriteLabel";
            this.fileTypeWriteLabel.Size = new System.Drawing.Size(72, 16);
            this.fileTypeWriteLabel.TabIndex = 0;
            this.fileTypeWriteLabel.Text = "File Type:";
            // 
            // saveStatisticsCheckBox
            // 
            this.saveStatisticsCheckBox.AutoSize = true;
            this.saveStatisticsCheckBox.Checked = true;
            this.saveStatisticsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveStatisticsCheckBox.Location = new System.Drawing.Point(60, 52);
            this.saveStatisticsCheckBox.Name = "saveStatisticsCheckBox";
            this.saveStatisticsCheckBox.Size = new System.Drawing.Size(96, 17);
            this.saveStatisticsCheckBox.TabIndex = 9;
            this.saveStatisticsCheckBox.Text = "Save Statistics";
            this.saveStatisticsCheckBox.UseVisualStyleBackColor = true;
            this.saveStatisticsCheckBox.CheckedChanged += new System.EventHandler(this.saveTimestampCheckBox_CheckedChanged);
            // 
            // triggerParametersGroupBox
            // 
            this.triggerParametersGroupBox.Controls.Add(this.pauseTriggerPanel);
            this.triggerParametersGroupBox.Controls.Add(this.pauseTriggerLabel);
            this.triggerParametersGroupBox.Controls.Add(this.triggerSourceLabel);
            this.triggerParametersGroupBox.Controls.Add(this.triggerSourceTextBox);
            this.triggerParametersGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.triggerParametersGroupBox.Location = new System.Drawing.Point(8, 296);
            this.triggerParametersGroupBox.Name = "triggerParametersGroupBox";
            this.triggerParametersGroupBox.Size = new System.Drawing.Size(224, 86);
            this.triggerParametersGroupBox.TabIndex = 8;
            this.triggerParametersGroupBox.TabStop = false;
            this.triggerParametersGroupBox.Text = "Pause Trigger Parameters";
            // 
            // pauseTriggerPanel
            // 
            this.pauseTriggerPanel.Controls.Add(this.pauseWhenLowButton);
            this.pauseTriggerPanel.Controls.Add(this.pauseWhenHighButton);
            this.pauseTriggerPanel.Enabled = false;
            this.pauseTriggerPanel.Location = new System.Drawing.Point(98, 50);
            this.pauseTriggerPanel.Name = "pauseTriggerPanel";
            this.pauseTriggerPanel.Size = new System.Drawing.Size(126, 30);
            this.pauseTriggerPanel.TabIndex = 8;
            // 
            // pauseWhenLowButton
            // 
            this.pauseWhenLowButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.pauseWhenLowButton.Location = new System.Drawing.Point(73, 4);
            this.pauseWhenLowButton.Name = "pauseWhenLowButton";
            this.pauseWhenLowButton.Size = new System.Drawing.Size(45, 25);
            this.pauseWhenLowButton.TabIndex = 1;
            this.pauseWhenLowButton.Text = "Low";
            this.pauseWhenLowButton.CheckedChanged += new System.EventHandler(this.triggerButton_CheckedChanged);
            // 
            // pauseWhenHighButton
            // 
            this.pauseWhenHighButton.Checked = true;
            this.pauseWhenHighButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.pauseWhenHighButton.Location = new System.Drawing.Point(13, 4);
            this.pauseWhenHighButton.Name = "pauseWhenHighButton";
            this.pauseWhenHighButton.Size = new System.Drawing.Size(45, 25);
            this.pauseWhenHighButton.TabIndex = 0;
            this.pauseWhenHighButton.TabStop = true;
            this.pauseWhenHighButton.Text = "High";
            this.pauseWhenHighButton.CheckedChanged += new System.EventHandler(this.triggerButton_CheckedChanged);
            // 
            // pauseTriggerLabel
            // 
            this.pauseTriggerLabel.AutoSize = true;
            this.pauseTriggerLabel.Location = new System.Drawing.Point(16, 58);
            this.pauseTriggerLabel.Name = "pauseTriggerLabel";
            this.pauseTriggerLabel.Size = new System.Drawing.Size(76, 13);
            this.pauseTriggerLabel.TabIndex = 2;
            this.pauseTriggerLabel.Text = "Pause Trigger:";
            // 
            // triggerSourceLabel
            // 
            this.triggerSourceLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.triggerSourceLabel.Location = new System.Drawing.Point(16, 25);
            this.triggerSourceLabel.Name = "triggerSourceLabel";
            this.triggerSourceLabel.Size = new System.Drawing.Size(48, 16);
            this.triggerSourceLabel.TabIndex = 0;
            this.triggerSourceLabel.Text = "Source:";
            // 
            // triggerSourceTextBox
            // 
            this.triggerSourceTextBox.Location = new System.Drawing.Point(120, 23);
            this.triggerSourceTextBox.Name = "triggerSourceTextBox";
            this.triggerSourceTextBox.Size = new System.Drawing.Size(96, 20);
            this.triggerSourceTextBox.TabIndex = 1;
            this.triggerSourceTextBox.TextChanged += new System.EventHandler(this.triggerSourceTextBox_TextChanged);
            // 
            // waveformChart
            // 
            chartArea1.Name = "ChartArea1";
            this.waveformChart.ChartAreas.Add(chartArea1);
            this.waveformChart.Location = new System.Drawing.Point(6, 19);
            this.waveformChart.Name = "waveformChart";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.waveformChart.Series.Add(series1);
            this.waveformChart.Size = new System.Drawing.Size(729, 300);
            this.waveformChart.TabIndex = 9;
            this.waveformChart.Text = "chart1";
            // 
            // horizontalScaleNumeric
            // 
            this.horizontalScaleNumeric.DecimalPlaces = 1;
            this.horizontalScaleNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.horizontalScaleNumeric.Location = new System.Drawing.Point(126, 23);
            this.horizontalScaleNumeric.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.horizontalScaleNumeric.Name = "horizontalScaleNumeric";
            this.horizontalScaleNumeric.Size = new System.Drawing.Size(90, 20);
            this.horizontalScaleNumeric.TabIndex = 10;
            this.horizontalScaleNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.horizontalScaleNumeric.ValueChanged += new System.EventHandler(this.horizontalScale_ValueChanged);
            // 
            // plotParametersBox
            // 
            this.plotParametersBox.Controls.Add(this.autoScaleCheckBox);
            this.plotParametersBox.Controls.Add(this.verticalScaleNumeric);
            this.plotParametersBox.Controls.Add(this.verticalScaleLabel);
            this.plotParametersBox.Controls.Add(this.plottingCheckBox);
            this.plotParametersBox.Controls.Add(this.horizontalScaleLabel);
            this.plotParametersBox.Controls.Add(this.horizontalScaleNumeric);
            this.plotParametersBox.Location = new System.Drawing.Point(8, 182);
            this.plotParametersBox.Name = "plotParametersBox";
            this.plotParametersBox.Size = new System.Drawing.Size(224, 108);
            this.plotParametersBox.TabIndex = 11;
            this.plotParametersBox.TabStop = false;
            this.plotParametersBox.Text = "Plot Parameters";
            // 
            // autoScaleCheckBox
            // 
            this.autoScaleCheckBox.AutoSize = true;
            this.autoScaleCheckBox.Checked = true;
            this.autoScaleCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoScaleCheckBox.Location = new System.Drawing.Point(19, 83);
            this.autoScaleCheckBox.Name = "autoScaleCheckBox";
            this.autoScaleCheckBox.Size = new System.Drawing.Size(78, 17);
            this.autoScaleCheckBox.TabIndex = 16;
            this.autoScaleCheckBox.Text = "Auto Scale";
            this.autoScaleCheckBox.UseVisualStyleBackColor = true;
            this.autoScaleCheckBox.CheckedChanged += new System.EventHandler(this.autoScaleCheckBox_CheckedChanged);
            // 
            // verticalScaleNumeric
            // 
            this.verticalScaleNumeric.Location = new System.Drawing.Point(126, 53);
            this.verticalScaleNumeric.Name = "verticalScaleNumeric";
            this.verticalScaleNumeric.Size = new System.Drawing.Size(90, 20);
            this.verticalScaleNumeric.TabIndex = 15;
            this.verticalScaleNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // verticalScaleLabel
            // 
            this.verticalScaleLabel.AutoSize = true;
            this.verticalScaleLabel.Location = new System.Drawing.Point(16, 55);
            this.verticalScaleLabel.Name = "verticalScaleLabel";
            this.verticalScaleLabel.Size = new System.Drawing.Size(91, 13);
            this.verticalScaleLabel.TabIndex = 14;
            this.verticalScaleLabel.Text = "Vertical Scale (V):";
            // 
            // plottingCheckBox
            // 
            this.plottingCheckBox.AutoSize = true;
            this.plottingCheckBox.Location = new System.Drawing.Point(120, 83);
            this.plottingCheckBox.Name = "plottingCheckBox";
            this.plottingCheckBox.Size = new System.Drawing.Size(81, 17);
            this.plottingCheckBox.TabIndex = 13;
            this.plottingCheckBox.Text = "Display Plot";
            this.plottingCheckBox.UseVisualStyleBackColor = true;
            this.plottingCheckBox.CheckedChanged += new System.EventHandler(this.plottingCheckBox_CheckedChanged);
            // 
            // horizontalScaleLabel
            // 
            this.horizontalScaleLabel.AutoSize = true;
            this.horizontalScaleLabel.Location = new System.Drawing.Point(16, 25);
            this.horizontalScaleLabel.Name = "horizontalScaleLabel";
            this.horizontalScaleLabel.Size = new System.Drawing.Size(101, 13);
            this.horizontalScaleLabel.TabIndex = 11;
            this.horizontalScaleLabel.Text = "Horizontal Scale (s):";
            // 
            // plotBox
            // 
            this.plotBox.Controls.Add(this.waveformChart);
            this.plotBox.Location = new System.Drawing.Point(238, 8);
            this.plotBox.Name = "plotBox";
            this.plotBox.Size = new System.Drawing.Size(741, 329);
            this.plotBox.TabIndex = 12;
            this.plotBox.TabStop = false;
            this.plotBox.Text = "Waveform Plot";
            // 
            // statisticsBox
            // 
            this.statisticsBox.Controls.Add(this.statisticsFrequencyNumeric);
            this.statisticsBox.Controls.Add(this.statisticsFrequencyLabel);
            this.statisticsBox.Controls.Add(this.saveStatisticsCheckBox);
            this.statisticsBox.Location = new System.Drawing.Point(8, 388);
            this.statisticsBox.Name = "statisticsBox";
            this.statisticsBox.Size = new System.Drawing.Size(224, 75);
            this.statisticsBox.TabIndex = 13;
            this.statisticsBox.TabStop = false;
            this.statisticsBox.Text = "Process Statistics";
            // 
            // statisticsFrequencyNumeric
            // 
            this.statisticsFrequencyNumeric.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.statisticsFrequencyNumeric.Location = new System.Drawing.Point(126, 23);
            this.statisticsFrequencyNumeric.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.statisticsFrequencyNumeric.Name = "statisticsFrequencyNumeric";
            this.statisticsFrequencyNumeric.Size = new System.Drawing.Size(90, 20);
            this.statisticsFrequencyNumeric.TabIndex = 11;
            this.statisticsFrequencyNumeric.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // statisticsFrequencyLabel
            // 
            this.statisticsFrequencyLabel.AutoSize = true;
            this.statisticsFrequencyLabel.Location = new System.Drawing.Point(16, 25);
            this.statisticsFrequencyLabel.Name = "statisticsFrequencyLabel";
            this.statisticsFrequencyLabel.Size = new System.Drawing.Size(82, 13);
            this.statisticsFrequencyLabel.TabIndex = 10;
            this.statisticsFrequencyLabel.Text = "Frequency (Hz):";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(991, 470);
            this.Controls.Add(this.statisticsBox);
            this.Controls.Add(this.plotBox);
            this.Controls.Add(this.plotParametersBox);
            this.Controls.Add(this.triggerParametersGroupBox);
            this.Controls.Add(this.writeToFileGroupBox);
            this.Controls.Add(this.timingParametersGroupBox);
            this.Controls.Add(this.channelParametersGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Photodiode Interface";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.channelParametersGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.minimumValueNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maximumValueNumeric)).EndInit();
            this.timingParametersGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rateNumeric)).EndInit();
            this.writeToFileGroupBox.ResumeLayout(false);
            this.writeToFileGroupBox.PerformLayout();
            this.saveFileTypePanel.ResumeLayout(false);
            this.triggerParametersGroupBox.ResumeLayout(false);
            this.triggerParametersGroupBox.PerformLayout();
            this.pauseTriggerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.waveformChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalScaleNumeric)).EndInit();
            this.plotParametersBox.ResumeLayout(false);
            this.plotParametersBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.verticalScaleNumeric)).EndInit();
            this.plotBox.ResumeLayout(false);
            this.statisticsBox.ResumeLayout(false);
            this.statisticsBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statisticsFrequencyNumeric)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.DoEvents();
            Application.Run(new MainForm());
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            waveformChart.Series[0].ChartType = SeriesChartType.Line;
            waveformChart.ChartAreas[0].AxisX.Minimum = 0;
            waveformChart.ChartAreas[0].AxisX.Maximum = (double)horizontalScaleNumeric.Value;
            waveformChart.ChartAreas[0].AxisX.Title = "Time (s)";
            waveformChart.ChartAreas[0].AxisY.Title = "Voltage (V)";

            verticalScaleNumeric.Minimum = minimumValueNumeric.Value;
            verticalScaleNumeric.Maximum = maximumValueNumeric.Value;
        }
        private void browseWriteButton_Click(object sender, System.EventArgs e)
        {
            useTextFileWrite = textFileWriteRadioButton.Checked;

            // Display Save File Dialog (Windows forms control)
            DialogResult result = outputFolderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                outputRootFolder = outputFolderDialog.SelectedPath;

                filePathWriteTextBox.Text = outputRootFolder;
                fileToolTip.SetToolTip(filePathWriteTextBox, outputRootFolder);
                startButton.Enabled = true;
            }
        }

        private void startButton_Click(object sender, System.EventArgs e)
        {
            if (!recordingEnabled)
            {
                // Create a new file for data
                CreateDataFile();

                // Modify the UI
                stopButton.Enabled = true;
                startButton.Enabled = false;
                statisticsBox.Enabled = false;
                saveFileTypePanel.Enabled = false;
                recordingEnabled = true;
                timingParametersGroupBox.Enabled = false;
                channelParametersGroupBox.Enabled = false;
                StartTask();
                PrepareFileForData();

                if (statisticsEnabled)
                {
                    acquisitionStart = DateTime.UtcNow;
                    totalSamplesAcquired = 0;

                    // Statistics timing
                    samplesPerStatisticsInterval =
                        (double)rateNumeric.Value /
                        (double)statisticsFrequencyNumeric.Value;

                    nextStatisticsSample = samplesPerStatisticsInterval;

                    // Reset accumulators
                    statisticsSamplesCollected = 0;
                    statisticsSum = 0;
                    statisticsSumSq = 0;
                    statisticsMin = double.MaxValue;
                    statisticsMax = double.MinValue;
                }
            }
        }

        private void StartTask()
        {
            if (runningTask == null)
            {
                try
                {
                    //Create a new task
                    myTask = new Task();

                    //Create a virtual channel
                    myTask.AIChannels.CreateVoltageChannel(physicalChannelComboBox.Text, "",
                        AITerminalConfiguration.Rse, Convert.ToDouble(minimumValueNumeric.Value),
                        Convert.ToDouble(maximumValueNumeric.Value), AIVoltageUnits.Volts);

                    //Configure the timing parameters
                    myTask.Timing.ConfigureSampleClock("", Convert.ToDouble(rateNumeric.Value),
                        SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, (int)rateNumeric.Value);

                    if (triggerSourceTextBox.Text != "")
                    {
                        // Configure Pause Trigger
                        myTask.Triggers.PauseTrigger.ConfigureDigitalLevelTrigger(triggerSourceTextBox.Text, gateLevel);
                    }

                    //Verify the Task
                    myTask.Control(TaskAction.Verify);

                    acquisitionStart = DateTime.UtcNow;
                    totalSamplesAcquired = 0;

                    runningTask = myTask;
                    analogInReader = new AnalogMultiChannelReader(myTask.Stream);

                    nextStatisticsSample = 0;

                    // Use SynchronizeCallbacks to specify that the object 
                    // marshals callbacks across threads appropriately.
                    analogInReader.SynchronizeCallbacks = true;

                    analogCallback = new AsyncCallback(AnalogInCallback);

                    analogInReader.BeginReadMultiSample((int)rateNumeric.Value, analogCallback, myTask);
                }
                catch (DaqException exception)
                {
                    //Display Errors
                    MessageBox.Show(exception.Message);
                    runningTask = null;
                    myTask.Dispose();
                    stopButton.Enabled = false;
                    startButton.Enabled = true;
                    statisticsBox.Enabled = true;
                    saveFileTypePanel.Enabled = true;
                    recordingEnabled = false;
                }
            }
        }
        private void StopTask()
        {
            if (runningTask != null)
            {
                try
                {
                    runningTask = null;
                    myTask.Dispose();
                }
                catch (DaqException exception)
                {
                    //Display Errors
                    MessageBox.Show(exception.Message);
                    runningTask = null;
                    myTask.Dispose();
                    stopButton.Enabled = false;
                    startButton.Enabled = true;
                    statisticsBox.Enabled = true;
                    saveFileTypePanel.Enabled = true;
                    recordingEnabled = false;
                }
            }
        }
        private void AnalogInCallback(IAsyncResult ar)
        {
            try
            {
                if (runningTask != null && runningTask == ar.AsyncState)
                {
                    //Read the available data from the channels
                    data = analogInReader.EndReadMultiSample(ar);
                    totalSamplesAcquired += data.GetLength(1);
                    //Plots data
                    if (plottingEnabled)
                    {
                        UpdatePlot(data);
                    }
                    else
                    {
                        waveformChart.Series[0].Points.Clear();
                    }

                    //Writes waveform to file
                    if (recordingEnabled)
                    {
                        WriteDataToFile(data);

                        //Writes statistics to file
                        if (statisticsEnabled)
                        {
                            WriteStatisticsToFile(data);
                        }
                    }
                    
                    analogInReader.BeginReadMultiSample(Convert.ToInt32(rateNumeric.Value),analogCallback, myTask);
                }
            }
            catch (DaqException exception)
            {
                //Display Errors
                MessageBox.Show(exception.Message);
                runningTask = null;
                myTask.Dispose();
                stopButton.Enabled = false;
                startButton.Enabled = true;
                recordingEnabled = false;
            }
        }
        private void UpdatePlot(double[,] data)
        {
            int rate = (int)rateNumeric.Value;

            // Number of samples visible on screen
            int visibleSamples = (int)Math.Round(rate * (double)horizontalScaleNumeric.Value) + 1;

            // Keep 10 seconds of history minimum
            int maxHistorySamples = Math.Max(rate * 10, visibleSamples);

            // Add new samples to history buffer
            for (int i = 0; i < data.GetLength(1); i++)
            {
                plotHistory.Enqueue(data[0, i]);

                while (plotHistory.Count > maxHistorySamples)
                    plotHistory.Dequeue();
            }

            // Limit redraw rate
            if ((DateTime.Now - lastPlotUpdate).TotalMilliseconds < 100)
                return;

            lastPlotUpdate = DateTime.Now;

            // Determine visible range
            int availableSamples = plotHistory.Count;

            int samplesToShow = Math.Min(visibleSamples, availableSamples);

            if (samplesToShow == 0)
                return;

            // Get latest samples only
            double[] history = plotHistory.ToArray();

            int startIndex = availableSamples - samplesToShow;

            // Downsample for rendering
            int maxDisplayPoints = 1000;

            int stride = Math.Max(1, samplesToShow / maxDisplayPoints);

            // Update axis
            waveformChart.ChartAreas[0].AxisX.Minimum = (-(double)samplesToShow + 1) / rate;

            waveformChart.ChartAreas[0].AxisX.Maximum = 0;

            var series = waveformChart.Series[0];
            series.Points.Clear();

            for (int i = startIndex; i < availableSamples; i += stride)
            {
                int samplesBehind = availableSamples - 1 - i;

                double t = -(double)samplesBehind / rate;

                series.Points.AddXY(t, history[i]);
            }
            Console.WriteLine(
    $"visibleSamples={visibleSamples}, " +
    $"axisMin={waveformChart.ChartAreas[0].AxisX.Minimum}, " +
    $"firstPoint={series.Points[0].XValue}, " +
    $"lastPoint={series.Points[series.Points.Count - 1].XValue}"
);
        }
        private void WriteStatisticsToFile(double[,] data)
        {
            int samplesInCallback = data.GetLength(1);

            // Sample number where this callback begins
            long callbackStartSample =
                totalSamplesAcquired - samplesInCallback;

            for (int i = 0; i < samplesInCallback; i++)
            {
                //------------------------------------------
                // Absolute sample index in acquisition
                //------------------------------------------

                long currentSample = callbackStartSample + i + 1;

                double v = data[0, i];

                //------------------------------------------
                // Accumulate statistics
                //------------------------------------------

                statisticsSamplesCollected++;

                statisticsSum += v;
                statisticsSumSq += v * v;

                if (v < statisticsMin)
                    statisticsMin = v;

                if (v > statisticsMax)
                    statisticsMax = v;

                //------------------------------------------
                // Check if we've crossed boundary
                //------------------------------------------

                if (currentSample >= nextStatisticsSample)
                {
                    //--------------------------------------
                    // Calculate statistics
                    //--------------------------------------

                    double mean =
                        statisticsSum / statisticsSamplesCollected;

                    double rms =
                        Math.Sqrt(
                            statisticsSumSq /
                            statisticsSamplesCollected);

                    //--------------------------------------
                    // Timestamp
                    //--------------------------------------

                    double elapsedSeconds =
                        currentSample /
                        (double)rateNumeric.Value;

                    double timestamp =
                        acquisitionStart
                            .AddSeconds(elapsedSeconds)
                            .ToOADate();

                    //--------------------------------------
                    // Write to file
                    //--------------------------------------

                    timestampWriter.WriteLine(
                        $"{timestamp:F10}," +
                        $"{elapsedSeconds:F6}," +
                        $"{mean:F6}," +
                        $"{statisticsMin:F6}," +
                        $"{statisticsMax:F6}," +
                        $"{rms:F6}");

                    //--------------------------------------
                    // Reset accumulator
                    //--------------------------------------

                    statisticsSamplesCollected = 0;

                    statisticsSum = 0;
                    statisticsSumSq = 0;

                    statisticsMin = double.MaxValue;
                    statisticsMax = double.MinValue;

                    //--------------------------------------
                    // Schedule next boundary
                    //--------------------------------------

                    nextStatisticsSample +=
                        samplesPerStatisticsInterval;
                }
            }

            timestampWriter.Flush();
        }
        private void stopButton_Click(object sender, System.EventArgs e)
        {
            if (!plottingEnabled)
            {
                StopTask();
            }
            CloseFile();
            recordingEnabled = false;
            stopButton.Enabled = false;
            startButton.Enabled = true;
            statisticsBox.Enabled = true;
            timingParametersGroupBox.Enabled = true;
            channelParametersGroupBox.Enabled = true;
            saveFileTypePanel.Enabled = true;
        }
        private void triggerButton_CheckedChanged(object sender, System.EventArgs e)
        {
            if (pauseWhenHighButton.Checked)
                gateLevel = DigitalLevelPauseTriggerCondition.High;
            else
                gateLevel = DigitalLevelPauseTriggerCondition.Low;
        }
        private void WriteDataToFile(double[,] data)
        {
            int channels = data.GetLength(0);
            int samples = data.GetLength(1);

            if (useTextFileWrite)
            {
                for (int i = 0; i < samples; i++)
                {
                    for (int ch = 0; ch < channels; ch++)
                    {
                        fileStreamWriter.Write(
                            data[ch, i].ToString("E6"));

                        if (ch < channels - 1)
                            fileStreamWriter.Write('\t');
                    }

                    fileStreamWriter.WriteLine();
                }

                fileStreamWriter.Flush();
            }
            else
            {
                for (int i = 0; i < samples; i++)
                {
                    for (int ch = 0; ch < channels; ch++)
                    {
                        fileBinaryWriter.Write(data[ch, i]);
                    }
                }

                fileBinaryWriter.Flush();
            }
        }
        private void CloseFile()
        {
            int channelCount = savedData.Count;
            int dataCount = (savedData[0] as ArrayList).Count;

            try
            {
                if (useTextFileWrite)
                    fileStreamWriter.Close();
                else
                    fileBinaryWriter.Close();

                if (timestampWriter != null)
                {
                    timestampWriter.Close();
                    timestampWriter = null;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.TargetSite.ToString());
                runningTask = null;
                myTask.Dispose();
                stopButton.Enabled = false;
                startButton.Enabled = true;
                recordingEnabled = false;
            }
        }

        //Creates a text/binary stream based on the user selections
        private bool CreateDataFile()
        {
            try
            {
                string timestamp = DateTime.Now.ToString("Photodiode_yyyy-MMdd_HHmmss");

                sessionFolder = Path.Combine(outputRootFolder, timestamp);
                Directory.CreateDirectory(sessionFolder);
                string waveformFile = Path.Combine(sessionFolder, useTextFileWrite ? "acquisitionData.txt" : "acquisitionData.bin");
                string statisticsFile = Path.Combine(sessionFolder, "processStatistics.csv");
                FileStream fs = new FileStream(waveformFile, FileMode.Create, FileAccess.Write, FileShare.Read);

                if (useTextFileWrite)
                    fileStreamWriter = new StreamWriter(fs);
                else
                    fileBinaryWriter = new BinaryWriter(fs);

                timestampWriter = new StreamWriter(statisticsFile);
                timestampWriter.WriteLine("Timestamp,ElapsedSeconds,MeanVoltage,MinVoltage,MaxVoltage,RMSVoltage");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        // Only used by text files to write the channel name
        // Can expand this for binary too
        private void PrepareFileForData()
        {
            //Prepare the table and file for Data
            String[] channelNames = new String[myTask.AIChannels.Count];
            int i = 0;
            foreach (AIChannel a in myTask.AIChannels)
            {
                channelNames[i++] = a.PhysicalName;
            }

            // Add the channel names (and any other information) to the file
            savedData = new ArrayList();
            for (i = 0; i < myTask.AIChannels.Count; i++)
            {
                savedData.Add(new ArrayList());
            }
            //Prepare file for data (Write out the channel names
            int numChannels = myTask.AIChannels.Count;

            if (useTextFileWrite)
            {
                for (int j = 0; j < numChannels; j++)
                {   
                    fileStreamWriter.Write(myTask.AIChannels[j].PhysicalName);
                    fileStreamWriter.Write("\t"); 
                }
                fileStreamWriter.WriteLine();
            }
            else
            {
                for (int j = 0; j < numChannels; j++)
                {   
                    fileBinaryWriter.Write(myTask.AIChannels[j].PhysicalName);
                }
                fileBinaryWriter.Write("\r\n");
            }
        }
        private void fileWriteRadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            if (textFileWriteRadioButton.Checked)
            {
                useTextFileWrite = true;
            }
            else if (binaryFileWriteRadioButton.Checked)
            {
                useTextFileWrite = false;
            }

            startButton.Enabled = false;
        }

        private void horizontalScale_ValueChanged(object sender, EventArgs e)
        {
            waveformChart.ChartAreas[0].AxisX.Maximum = (double)horizontalScaleNumeric.Value;
        }

        private void triggerSourceTextBox_TextChanged(object sender, EventArgs e)
        {
            if (triggerSourceTextBox.Text != "")
                pauseTriggerPanel.Enabled = true;
            else
                pauseTriggerPanel.Enabled = false;
        }
        private void saveTimestampCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            statisticsEnabled = saveStatisticsCheckBox.Checked;
        }

        private void autoScaleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoScaleCheckBox.Checked)
            {
                this.waveformChart.ChartAreas[0].AxisY.Minimum = Double.NaN;
                this.waveformChart.ChartAreas[0].AxisY.Maximum = Double.NaN;
                this.waveformChart.ChartAreas[0].RecalculateAxesScale();
            }
            else
            {
                this.waveformChart.ChartAreas[0].AxisY.Minimum = (double)minimumValueNumeric.Value;
                this.waveformChart.ChartAreas[0].AxisY.Maximum = (double)maximumValueNumeric.Value;
            }
        }

        private void plottingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            plottingEnabled = plottingCheckBox.Checked;
            if (runningTask == null && plottingEnabled)
            {
                StartTask();
            }
            else if (runningTask != null && !plottingEnabled && !recordingEnabled)
            {
                StopTask();
            }
        }

        private void verticalScaleNumeric_ValueChanged(object sender, EventArgs e)
        {
            verticalScaleNumeric.Minimum = minimumValueNumeric.Value;
            verticalScaleNumeric.Maximum = maximumValueNumeric.Value;
        }

        private void rateNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (runningTask != null)
            {
                myTask.Stop();
                myTask.Timing.ConfigureSampleClock("", Convert.ToDouble(rateNumeric.Value),
                        SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, (int)rateNumeric.Value);
                myTask.Start();
            }
            
            
        }
    }
}
