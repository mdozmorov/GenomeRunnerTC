<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormExonGenerator
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormExonGenerator))
        Me.btnRun = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lblProgress = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ComboBoxExonEnd = New System.Windows.Forms.ComboBox()
        Me.ComboBoxExonStart = New System.Windows.Forms.ComboBox()
        Me.lbl1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnSkip = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnRun
        '
        Me.btnRun.Location = New System.Drawing.Point(12, 113)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(190, 23)
        Me.btnRun.TabIndex = 0
        Me.btnRun.Text = "Create Exon Table for Feature"
        Me.btnRun.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(12, 171)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(429, 23)
        Me.ProgressBar1.TabIndex = 1
        '
        'lblProgress
        '
        Me.lblProgress.AutoSize = True
        Me.lblProgress.Location = New System.Drawing.Point(12, 155)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(0, 13)
        Me.lblProgress.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(429, 43)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'ComboBoxExonEnd
        '
        Me.ComboBoxExonEnd.FormattingEnabled = True
        Me.ComboBoxExonEnd.Location = New System.Drawing.Point(299, 73)
        Me.ComboBoxExonEnd.Name = "ComboBoxExonEnd"
        Me.ComboBoxExonEnd.Size = New System.Drawing.Size(142, 21)
        Me.ComboBoxExonEnd.TabIndex = 4
        '
        'ComboBoxExonStart
        '
        Me.ComboBoxExonStart.FormattingEnabled = True
        Me.ComboBoxExonStart.Location = New System.Drawing.Point(71, 73)
        Me.ComboBoxExonStart.Name = "ComboBoxExonStart"
        Me.ComboBoxExonStart.Size = New System.Drawing.Size(142, 21)
        Me.ComboBoxExonStart.TabIndex = 5
        '
        'lbl1
        '
        Me.lbl1.AutoSize = True
        Me.lbl1.Location = New System.Drawing.Point(240, 76)
        Me.lbl1.Name = "lbl1"
        Me.lbl1.Size = New System.Drawing.Size(53, 13)
        Me.lbl1.TabIndex = 6
        Me.lbl1.Text = "Exon End"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 76)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(56, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Exon Start"
        '
        'btnSkip
        '
        Me.btnSkip.Location = New System.Drawing.Point(251, 113)
        Me.btnSkip.Name = "btnSkip"
        Me.btnSkip.Size = New System.Drawing.Size(190, 23)
        Me.btnSkip.TabIndex = 8
        Me.btnSkip.Text = "Skip"
        Me.btnSkip.UseVisualStyleBackColor = True
        '
        'FormExonGenerator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(453, 206)
        Me.Controls.Add(Me.btnSkip)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lbl1)
        Me.Controls.Add(Me.ComboBoxExonStart)
        Me.Controls.Add(Me.ComboBoxExonEnd)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblProgress)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.btnRun)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "FormExonGenerator"
        Me.ShowIcon = False
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents lblProgress As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxExonEnd As System.Windows.Forms.ComboBox
    Friend WithEvents ComboBoxExonStart As System.Windows.Forms.ComboBox
    Friend WithEvents lbl1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnSkip As System.Windows.Forms.Button

End Class
