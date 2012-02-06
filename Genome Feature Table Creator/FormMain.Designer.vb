<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.txtUser = New System.Windows.Forms.TextBox()
        Me.txtHost = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtDatabase = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lblProgress = New System.Windows.Forms.Label()
        Me.listFeaturesAvailable = New System.Windows.Forms.ListBox()
        Me.btnReloadFeatures = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.listFeaturesToAdd = New System.Windows.Forms.ListBox()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnMakeDatabase = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnStatistics = New System.Windows.Forms.Button()
        Me.btnGetCountofBasePairsCovered = New System.Windows.Forms.Button()
        Me.btnGenerateBedFile = New System.Windows.Forms.Button()
        Me.btnExonTable = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SaveFD = New System.Windows.Forms.SaveFileDialog()
        Me.txtUcscdb = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.btnCompareToUCSC = New System.Windows.Forms.Button()
        Me.btnLoadGRTable = New System.Windows.Forms.Button()
        Me.btnUpdateStatistics = New System.Windows.Forms.Button()
        Me.btnExportGenomeRunner = New System.Windows.Forms.Button()
        Me.lblDataDownloadPath = New System.Windows.Forms.Label()
        Me.btnChangeDataDownloadPath = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtUser
        '
        Me.txtUser.Location = New System.Drawing.Point(61, 58)
        Me.txtUser.Name = "txtUser"
        Me.txtUser.Size = New System.Drawing.Size(117, 20)
        Me.txtUser.TabIndex = 3
        Me.txtUser.Text = "root"
        '
        'txtHost
        '
        Me.txtHost.Location = New System.Drawing.Point(61, 6)
        Me.txtHost.Name = "txtHost"
        Me.txtHost.Size = New System.Drawing.Size(117, 20)
        Me.txtHost.TabIndex = 1
        Me.txtHost.Text = "localhost"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(26, 61)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(29, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "User"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(2, 87)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Password"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(26, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Host"
        '
        'txtDatabase
        '
        Me.txtDatabase.Location = New System.Drawing.Point(61, 32)
        Me.txtDatabase.Name = "txtDatabase"
        Me.txtDatabase.Size = New System.Drawing.Size(117, 20)
        Me.txtDatabase.TabIndex = 2
        Me.txtDatabase.Text = "temp18"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(2, 35)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(53, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Database"
        '
        'btnConnect
        '
        Me.btnConnect.Location = New System.Drawing.Point(5, 136)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(173, 23)
        Me.btnConnect.TabIndex = 6
        Me.btnConnect.Text = "Connect"
        Me.btnConnect.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(7, 352)
        Me.ProgressBar1.Margin = New System.Windows.Forms.Padding(2)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(544, 19)
        Me.ProgressBar1.TabIndex = 36
        '
        'lblProgress
        '
        Me.lblProgress.AutoSize = True
        Me.lblProgress.Location = New System.Drawing.Point(4, 337)
        Me.lblProgress.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(66, 13)
        Me.lblProgress.TabIndex = 35
        Me.lblProgress.Text = "Progress bar"
        '
        'listFeaturesAvailable
        '
        Me.listFeaturesAvailable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.listFeaturesAvailable.FormattingEnabled = True
        Me.listFeaturesAvailable.Location = New System.Drawing.Point(640, 22)
        Me.listFeaturesAvailable.Name = "listFeaturesAvailable"
        Me.listFeaturesAvailable.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.listFeaturesAvailable.Size = New System.Drawing.Size(224, 199)
        Me.listFeaturesAvailable.Sorted = True
        Me.listFeaturesAvailable.TabIndex = 37
        '
        'btnReloadFeatures
        '
        Me.btnReloadFeatures.Location = New System.Drawing.Point(640, 274)
        Me.btnReloadFeatures.Name = "btnReloadFeatures"
        Me.btnReloadFeatures.Size = New System.Drawing.Size(197, 23)
        Me.btnReloadFeatures.TabIndex = 38
        Me.btnReloadFeatures.Text = "Update Features List"
        Me.btnReloadFeatures.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(637, 6)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(128, 13)
        Me.Label5.TabIndex = 40
        Me.Label5.Text = "Features Available to Add"
        '
        'listFeaturesToAdd
        '
        Me.listFeaturesToAdd.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.listFeaturesToAdd.FormattingEnabled = True
        Me.listFeaturesToAdd.HorizontalScrollbar = True
        Me.listFeaturesToAdd.Location = New System.Drawing.Point(184, 50)
        Me.listFeaturesToAdd.Name = "listFeaturesToAdd"
        Me.listFeaturesToAdd.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.listFeaturesToAdd.Size = New System.Drawing.Size(224, 277)
        Me.listFeaturesToAdd.TabIndex = 47
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(61, 84)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(117, 20)
        Me.txtPassword.TabIndex = 4
        Me.txtPassword.Text = "gamma123"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(184, 6)
        Me.Label6.MaximumSize = New System.Drawing.Size(250, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(231, 39)
        Me.Label6.TabIndex = 52
        Me.Label6.Text = "List of Features: Features to be Added (purple), Features in database (black).  E" & _
    "mpty Features (orange). Features to update (green)."
        '
        'btnMakeDatabase
        '
        Me.btnMakeDatabase.Location = New System.Drawing.Point(5, 285)
        Me.btnMakeDatabase.Name = "btnMakeDatabase"
        Me.btnMakeDatabase.Size = New System.Drawing.Size(173, 23)
        Me.btnMakeDatabase.TabIndex = 53
        Me.btnMakeDatabase.Text = "Make database"
        Me.btnMakeDatabase.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnCompareToUCSC)
        Me.GroupBox2.Controls.Add(Me.btnUpdateStatistics)
        Me.GroupBox2.Controls.Add(Me.btnGenerateBedFile)
        Me.GroupBox2.Location = New System.Drawing.Point(421, 6)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(210, 173)
        Me.GroupBox2.TabIndex = 57
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Special Tools"
        '
        'btnStatistics
        '
        Me.btnStatistics.Location = New System.Drawing.Point(640, 326)
        Me.btnStatistics.Name = "btnStatistics"
        Me.btnStatistics.Size = New System.Drawing.Size(197, 23)
        Me.btnStatistics.TabIndex = 61
        Me.btnStatistics.Text = "Select Statistics"
        Me.ToolTip1.SetToolTip(Me.btnStatistics, "Select which column to use as the threshold for the selected features")
        Me.btnStatistics.UseVisualStyleBackColor = True
        '
        'btnGetCountofBasePairsCovered
        '
        Me.btnGetCountofBasePairsCovered.Location = New System.Drawing.Point(640, 352)
        Me.btnGetCountofBasePairsCovered.Name = "btnGetCountofBasePairsCovered"
        Me.btnGetCountofBasePairsCovered.Size = New System.Drawing.Size(197, 23)
        Me.btnGetCountofBasePairsCovered.TabIndex = 62
        Me.btnGetCountofBasePairsCovered.Text = "Get count of base pairs covered by features"
        Me.btnGetCountofBasePairsCovered.UseVisualStyleBackColor = True
        '
        'btnGenerateBedFile
        '
        Me.btnGenerateBedFile.Enabled = False
        Me.btnGenerateBedFile.Location = New System.Drawing.Point(11, 81)
        Me.btnGenerateBedFile.Name = "btnGenerateBedFile"
        Me.btnGenerateBedFile.Size = New System.Drawing.Size(173, 23)
        Me.btnGenerateBedFile.TabIndex = 60
        Me.btnGenerateBedFile.Text = "Generate Feature Bed File "
        Me.ToolTip1.SetToolTip(Me.btnGenerateBedFile, "Extracts the Chrom, ChromStart, and ChromEnd of the selected feature and generate" & _
        "s a .bed file")
        Me.btnGenerateBedFile.UseVisualStyleBackColor = True
        '
        'btnExonTable
        '
        Me.btnExonTable.Enabled = False
        Me.btnExonTable.Location = New System.Drawing.Point(640, 300)
        Me.btnExonTable.Name = "btnExonTable"
        Me.btnExonTable.Size = New System.Drawing.Size(197, 23)
        Me.btnExonTable.TabIndex = 58
        Me.btnExonTable.Text = "Generate Exon Table"
        Me.ToolTip1.SetToolTip(Me.btnExonTable, "Generates an exon table from the KnownGene table in the mysql database")
        Me.btnExonTable.UseVisualStyleBackColor = True
        '
        'txtUcscdb
        '
        Me.txtUcscdb.Location = New System.Drawing.Point(61, 110)
        Me.txtUcscdb.Name = "txtUcscdb"
        Me.txtUcscdb.Size = New System.Drawing.Size(117, 20)
        Me.txtUcscdb.TabIndex = 5
        Me.txtUcscdb.Text = "hg18"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(4, 113)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(51, 13)
        Me.Label7.TabIndex = 59
        Me.Label7.Text = "UCSC db"
        '
        'btnCompareToUCSC
        '
        Me.btnCompareToUCSC.Location = New System.Drawing.Point(11, 19)
        Me.btnCompareToUCSC.Name = "btnCompareToUCSC"
        Me.btnCompareToUCSC.Size = New System.Drawing.Size(173, 23)
        Me.btnCompareToUCSC.TabIndex = 60
        Me.btnCompareToUCSC.Text = "Compare to UCSC"
        Me.btnCompareToUCSC.UseVisualStyleBackColor = True
        '
        'btnLoadGRTable
        '
        Me.btnLoadGRTable.Location = New System.Drawing.Point(5, 222)
        Me.btnLoadGRTable.Name = "btnLoadGRTable"
        Me.btnLoadGRTable.Size = New System.Drawing.Size(173, 23)
        Me.btnLoadGRTable.TabIndex = 61
        Me.btnLoadGRTable.Text = "Load GR table"
        Me.btnLoadGRTable.UseVisualStyleBackColor = True
        '
        'btnUpdateStatistics
        '
        Me.btnUpdateStatistics.Location = New System.Drawing.Point(11, 52)
        Me.btnUpdateStatistics.Name = "btnUpdateStatistics"
        Me.btnUpdateStatistics.Size = New System.Drawing.Size(173, 23)
        Me.btnUpdateStatistics.TabIndex = 62
        Me.btnUpdateStatistics.Text = "Update Statistics"
        Me.btnUpdateStatistics.UseVisualStyleBackColor = True
        '
        'btnExportGenomeRunner
        '
        Me.btnExportGenomeRunner.Location = New System.Drawing.Point(5, 251)
        Me.btnExportGenomeRunner.Name = "btnExportGenomeRunner"
        Me.btnExportGenomeRunner.Size = New System.Drawing.Size(173, 23)
        Me.btnExportGenomeRunner.TabIndex = 63
        Me.btnExportGenomeRunner.Text = "Export GenomeRunner"
        Me.btnExportGenomeRunner.UseVisualStyleBackColor = True
        '
        'lblDataDownloadPath
        '
        Me.lblDataDownloadPath.AutoSize = True
        Me.lblDataDownloadPath.Location = New System.Drawing.Point(4, 173)
        Me.lblDataDownloadPath.Name = "lblDataDownloadPath"
        Me.lblDataDownloadPath.Size = New System.Drawing.Size(106, 13)
        Me.lblDataDownloadPath.TabIndex = 64
        Me.lblDataDownloadPath.Text = "Data Download Path"
        '
        'btnChangeDataDownloadPath
        '
        Me.btnChangeDataDownloadPath.Location = New System.Drawing.Point(5, 193)
        Me.btnChangeDataDownloadPath.Name = "btnChangeDataDownloadPath"
        Me.btnChangeDataDownloadPath.Size = New System.Drawing.Size(173, 23)
        Me.btnChangeDataDownloadPath.TabIndex = 65
        Me.btnChangeDataDownloadPath.Text = "Change Data Download Path"
        Me.btnChangeDataDownloadPath.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(556, 352)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 66
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(684, 385)
        Me.Controls.Add(Me.btnStatistics)
        Me.Controls.Add(Me.btnGetCountofBasePairsCovered)
        Me.Controls.Add(Me.btnExonTable)
        Me.Controls.Add(Me.btnMakeDatabase)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnChangeDataDownloadPath)
        Me.Controls.Add(Me.lblDataDownloadPath)
        Me.Controls.Add(Me.btnExportGenomeRunner)
        Me.Controls.Add(Me.btnLoadGRTable)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtUcscdb)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.listFeaturesToAdd)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.btnReloadFeatures)
        Me.Controls.Add(Me.listFeaturesAvailable)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.lblProgress)
        Me.Controls.Add(Me.btnConnect)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtDatabase)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtHost)
        Me.Controls.Add(Me.txtPassword)
        Me.Controls.Add(Me.txtUser)
        Me.Name = "FormMain"
        Me.Text = "Form1"
        Me.GroupBox2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtUser As System.Windows.Forms.TextBox
    Friend WithEvents txtHost As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtDatabase As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents lblProgress As System.Windows.Forms.Label
    Friend WithEvents listFeaturesAvailable As System.Windows.Forms.ListBox
    Friend WithEvents btnReloadFeatures As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents listFeaturesToAdd As System.Windows.Forms.ListBox
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btnMakeDatabase As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents btnExonTable As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents btnGenerateBedFile As System.Windows.Forms.Button
    Friend WithEvents SaveFD As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnStatistics As System.Windows.Forms.Button
    Friend WithEvents btnGetCountofBasePairsCovered As System.Windows.Forms.Button
    Friend WithEvents txtUcscdb As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents btnCompareToUCSC As System.Windows.Forms.Button
    Friend WithEvents btnLoadGRTable As System.Windows.Forms.Button
    Friend WithEvents btnUpdateStatistics As System.Windows.Forms.Button
    Friend WithEvents btnExportGenomeRunner As System.Windows.Forms.Button
    Friend WithEvents lblDataDownloadPath As System.Windows.Forms.Label
    Friend WithEvents btnChangeDataDownloadPath As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button

End Class
