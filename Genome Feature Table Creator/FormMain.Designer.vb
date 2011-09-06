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
        Me.btnAddToList = New System.Windows.Forms.Button()
        Me.btnRemoveFromList = New System.Windows.Forms.Button()
        Me.listFeaturesToAdd = New System.Windows.Forms.ListBox()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.btnLoadList = New System.Windows.Forms.Button()
        Me.btnExportList = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnPrepareFiles = New System.Windows.Forms.Button()
        Me.btnCreateTables = New System.Windows.Forms.Button()
        Me.btnLoadData = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnGetCountofBasePairsCovered = New System.Windows.Forms.Button()
        Me.btnStatistics = New System.Windows.Forms.Button()
        Me.btnGenerateBedFile = New System.Windows.Forms.Button()
        Me.btnGenerateBackground = New System.Windows.Forms.Button()
        Me.btnExonTable = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SaveFD = New System.Windows.Forms.SaveFileDialog()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtUser
        '
        Me.txtUser.Location = New System.Drawing.Point(61, 58)
        Me.txtUser.Name = "txtUser"
        Me.txtUser.Size = New System.Drawing.Size(117, 20)
        Me.txtUser.TabIndex = 2
        Me.txtUser.Text = "root"
        '
        'txtHost
        '
        Me.txtHost.Location = New System.Drawing.Point(61, 6)
        Me.txtHost.Name = "txtHost"
        Me.txtHost.Size = New System.Drawing.Size(117, 20)
        Me.txtHost.TabIndex = 4
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
        Me.txtDatabase.TabIndex = 8
        Me.txtDatabase.Text = "hg18test"
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
        Me.btnConnect.Location = New System.Drawing.Point(61, 110)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(117, 23)
        Me.btnConnect.TabIndex = 10
        Me.btnConnect.Text = "Connect"
        Me.btnConnect.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(9, 373)
        Me.ProgressBar1.Margin = New System.Windows.Forms.Padding(2)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(489, 19)
        Me.ProgressBar1.TabIndex = 36
        '
        'lblProgress
        '
        Me.lblProgress.AutoSize = True
        Me.lblProgress.Location = New System.Drawing.Point(6, 358)
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
        Me.listFeaturesAvailable.Location = New System.Drawing.Point(225, 25)
        Me.listFeaturesAvailable.Name = "listFeaturesAvailable"
        Me.listFeaturesAvailable.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.listFeaturesAvailable.Size = New System.Drawing.Size(224, 329)
        Me.listFeaturesAvailable.Sorted = True
        Me.listFeaturesAvailable.TabIndex = 37
        '
        'btnReloadFeatures
        '
        Me.btnReloadFeatures.Location = New System.Drawing.Point(61, 139)
        Me.btnReloadFeatures.Name = "btnReloadFeatures"
        Me.btnReloadFeatures.Size = New System.Drawing.Size(117, 23)
        Me.btnReloadFeatures.TabIndex = 38
        Me.btnReloadFeatures.Text = "Update Features List"
        Me.btnReloadFeatures.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(222, 9)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(128, 13)
        Me.Label5.TabIndex = 40
        Me.Label5.Text = "Features Available to Add"
        '
        'btnAddToList
        '
        Me.btnAddToList.Location = New System.Drawing.Point(455, 134)
        Me.btnAddToList.Name = "btnAddToList"
        Me.btnAddToList.Size = New System.Drawing.Size(76, 23)
        Me.btnAddToList.TabIndex = 45
        Me.btnAddToList.Text = "Add ->"
        Me.btnAddToList.UseVisualStyleBackColor = True
        '
        'btnRemoveFromList
        '
        Me.btnRemoveFromList.Location = New System.Drawing.Point(455, 163)
        Me.btnRemoveFromList.Name = "btnRemoveFromList"
        Me.btnRemoveFromList.Size = New System.Drawing.Size(76, 23)
        Me.btnRemoveFromList.TabIndex = 46
        Me.btnRemoveFromList.Text = "<- Remove"
        Me.btnRemoveFromList.UseVisualStyleBackColor = True
        Me.btnRemoveFromList.Visible = False
        '
        'listFeaturesToAdd
        '
        Me.listFeaturesToAdd.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.listFeaturesToAdd.FormattingEnabled = True
        Me.listFeaturesToAdd.Location = New System.Drawing.Point(537, 52)
        Me.listFeaturesToAdd.Name = "listFeaturesToAdd"
        Me.listFeaturesToAdd.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.listFeaturesToAdd.Size = New System.Drawing.Size(224, 303)
        Me.listFeaturesToAdd.TabIndex = 47
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(61, 84)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(117, 20)
        Me.txtPassword.TabIndex = 3
        Me.txtPassword.Text = "gamma123"
        '
        'btnLoadList
        '
        Me.btnLoadList.Location = New System.Drawing.Point(537, 373)
        Me.btnLoadList.Name = "btnLoadList"
        Me.btnLoadList.Size = New System.Drawing.Size(101, 23)
        Me.btnLoadList.TabIndex = 49
        Me.btnLoadList.Text = "Load List"
        Me.btnLoadList.UseVisualStyleBackColor = True
        '
        'btnExportList
        '
        Me.btnExportList.Location = New System.Drawing.Point(644, 373)
        Me.btnExportList.Name = "btnExportList"
        Me.btnExportList.Size = New System.Drawing.Size(101, 23)
        Me.btnExportList.TabIndex = 50
        Me.btnExportList.Text = "Export List"
        Me.btnExportList.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(534, 6)
        Me.Label6.MaximumSize = New System.Drawing.Size(250, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(231, 39)
        Me.Label6.TabIndex = 52
        Me.Label6.Text = "List of Features: Features to be Added (purple), Features in database (black).  E" & _
            "mpty Features (orange)"
        '
        'btnPrepareFiles
        '
        Me.btnPrepareFiles.Location = New System.Drawing.Point(14, 19)
        Me.btnPrepareFiles.Name = "btnPrepareFiles"
        Me.btnPrepareFiles.Size = New System.Drawing.Size(116, 23)
        Me.btnPrepareFiles.TabIndex = 53
        Me.btnPrepareFiles.Text = "1. Prepare Files"
        Me.btnPrepareFiles.UseVisualStyleBackColor = True
        '
        'btnCreateTables
        '
        Me.btnCreateTables.Location = New System.Drawing.Point(13, 49)
        Me.btnCreateTables.Name = "btnCreateTables"
        Me.btnCreateTables.Size = New System.Drawing.Size(117, 23)
        Me.btnCreateTables.TabIndex = 54
        Me.btnCreateTables.Text = "2. Create Tables"
        Me.btnCreateTables.UseVisualStyleBackColor = True
        '
        'btnLoadData
        '
        Me.btnLoadData.Location = New System.Drawing.Point(14, 78)
        Me.btnLoadData.Name = "btnLoadData"
        Me.btnLoadData.Size = New System.Drawing.Size(117, 23)
        Me.btnLoadData.TabIndex = 55
        Me.btnLoadData.Text = "3. Populate Database"
        Me.btnLoadData.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnPrepareFiles)
        Me.GroupBox1.Controls.Add(Me.btnLoadData)
        Me.GroupBox1.Controls.Add(Me.btnCreateTables)
        Me.GroupBox1.Location = New System.Drawing.Point(767, 242)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(143, 112)
        Me.GroupBox1.TabIndex = 56
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Add Features"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnGetCountofBasePairsCovered)
        Me.GroupBox2.Controls.Add(Me.btnStatistics)
        Me.GroupBox2.Controls.Add(Me.btnGenerateBedFile)
        Me.GroupBox2.Controls.Add(Me.btnGenerateBackground)
        Me.GroupBox2.Controls.Add(Me.btnExonTable)
        Me.GroupBox2.Location = New System.Drawing.Point(9, 182)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(210, 173)
        Me.GroupBox2.TabIndex = 57
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Special Tools"
        '
        'btnGetCountofBasePairsCovered
        '
        Me.btnGetCountofBasePairsCovered.Location = New System.Drawing.Point(7, 144)
        Me.btnGetCountofBasePairsCovered.Name = "btnGetCountofBasePairsCovered"
        Me.btnGetCountofBasePairsCovered.Size = New System.Drawing.Size(197, 23)
        Me.btnGetCountofBasePairsCovered.TabIndex = 62
        Me.btnGetCountofBasePairsCovered.Text = "Get count of base pairs covered by features"
        Me.btnGetCountofBasePairsCovered.UseVisualStyleBackColor = True
        '
        'btnStatistics
        '
        Me.btnStatistics.Location = New System.Drawing.Point(7, 117)
        Me.btnStatistics.Name = "btnStatistics"
        Me.btnStatistics.Size = New System.Drawing.Size(197, 23)
        Me.btnStatistics.TabIndex = 61
        Me.btnStatistics.Text = "Select Threshold"
        Me.ToolTip1.SetToolTip(Me.btnStatistics, "Select which column to use as the threshold for the selected features")
        Me.btnStatistics.UseVisualStyleBackColor = True
        '
        'btnGenerateBedFile
        '
        Me.btnGenerateBedFile.Enabled = False
        Me.btnGenerateBedFile.Location = New System.Drawing.Point(7, 88)
        Me.btnGenerateBedFile.Name = "btnGenerateBedFile"
        Me.btnGenerateBedFile.Size = New System.Drawing.Size(197, 23)
        Me.btnGenerateBedFile.TabIndex = 60
        Me.btnGenerateBedFile.Text = "Generate Feature Bed File "
        Me.ToolTip1.SetToolTip(Me.btnGenerateBedFile, "Extracts the Chrom, ChromStart, and ChromEnd of the selected feature and generate" & _
                "s a .bed file")
        Me.btnGenerateBedFile.UseVisualStyleBackColor = True
        '
        'btnGenerateBackground
        '
        Me.btnGenerateBackground.Enabled = False
        Me.btnGenerateBackground.Location = New System.Drawing.Point(7, 58)
        Me.btnGenerateBackground.Name = "btnGenerateBackground"
        Me.btnGenerateBackground.Size = New System.Drawing.Size(197, 23)
        Me.btnGenerateBackground.TabIndex = 59
        Me.btnGenerateBackground.Text = "Generate Random Background"
        Me.ToolTip1.SetToolTip(Me.btnGenerateBackground, "Generates a random background from the selected feature")
        Me.btnGenerateBackground.UseVisualStyleBackColor = True
        '
        'btnExonTable
        '
        Me.btnExonTable.Enabled = False
        Me.btnExonTable.Location = New System.Drawing.Point(7, 28)
        Me.btnExonTable.Name = "btnExonTable"
        Me.btnExonTable.Size = New System.Drawing.Size(197, 23)
        Me.btnExonTable.TabIndex = 58
        Me.btnExonTable.Text = "Generate Exon Table"
        Me.ToolTip1.SetToolTip(Me.btnExonTable, "Generates an exon table from the KnownGene table in the mysql database")
        Me.btnExonTable.UseVisualStyleBackColor = True
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(909, 403)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.btnExportList)
        Me.Controls.Add(Me.btnLoadList)
        Me.Controls.Add(Me.listFeaturesToAdd)
        Me.Controls.Add(Me.btnRemoveFromList)
        Me.Controls.Add(Me.btnAddToList)
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
        Me.GroupBox1.ResumeLayout(False)
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
    Friend WithEvents btnAddToList As System.Windows.Forms.Button
    Friend WithEvents btnRemoveFromList As System.Windows.Forms.Button
    Friend WithEvents listFeaturesToAdd As System.Windows.Forms.ListBox
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents btnLoadList As System.Windows.Forms.Button
    Friend WithEvents btnExportList As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btnPrepareFiles As System.Windows.Forms.Button
    Friend WithEvents btnCreateTables As System.Windows.Forms.Button
    Friend WithEvents btnLoadData As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents btnExonTable As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents btnGenerateBackground As System.Windows.Forms.Button
    Friend WithEvents btnGenerateBedFile As System.Windows.Forms.Button
    Friend WithEvents SaveFD As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnStatistics As System.Windows.Forms.Button
    Friend WithEvents btnGetCountofBasePairsCovered As System.Windows.Forms.Button

End Class
