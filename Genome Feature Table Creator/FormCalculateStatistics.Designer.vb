<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormCalculateStatistics
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
        Me.ComboBoxColumns = New System.Windows.Forms.ComboBox()
        Me.btnSetAsTreshold = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblMin = New System.Windows.Forms.Label()
        Me.lblMax = New System.Windows.Forms.Label()
        Me.lblMean = New System.Windows.Forms.Label()
        Me.btnSkip = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnGenerateHistogram = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'ComboBoxColumns
        '
        Me.ComboBoxColumns.FormattingEnabled = True
        Me.ComboBoxColumns.Location = New System.Drawing.Point(96, 61)
        Me.ComboBoxColumns.Name = "ComboBoxColumns"
        Me.ComboBoxColumns.Size = New System.Drawing.Size(121, 21)
        Me.ComboBoxColumns.TabIndex = 0
        '
        'btnSetAsTreshold
        '
        Me.btnSetAsTreshold.Location = New System.Drawing.Point(12, 88)
        Me.btnSetAsTreshold.Name = "btnSetAsTreshold"
        Me.btnSetAsTreshold.Size = New System.Drawing.Size(147, 23)
        Me.btnSetAsTreshold.TabIndex = 1
        Me.btnSetAsTreshold.Text = "Use as Threshold"
        Me.btnSetAsTreshold.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(-1, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(333, 32)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Select the column to use as the threshold (MUST be of type integer)."
        '
        'lblMin
        '
        Me.lblMin.AutoSize = True
        Me.lblMin.Location = New System.Drawing.Point(12, 130)
        Me.lblMin.Name = "lblMin"
        Me.lblMin.Size = New System.Drawing.Size(69, 13)
        Me.lblMin.TabIndex = 3
        Me.lblMin.Text = "Minimum: NA"
        '
        'lblMax
        '
        Me.lblMax.AutoSize = True
        Me.lblMax.Location = New System.Drawing.Point(128, 130)
        Me.lblMax.Name = "lblMax"
        Me.lblMax.Size = New System.Drawing.Size(72, 13)
        Me.lblMax.TabIndex = 4
        Me.lblMax.Text = "Maximum: NA"
        '
        'lblMean
        '
        Me.lblMean.AutoSize = True
        Me.lblMean.Location = New System.Drawing.Point(249, 130)
        Me.lblMean.Name = "lblMean"
        Me.lblMean.Size = New System.Drawing.Size(55, 13)
        Me.lblMean.TabIndex = 5
        Me.lblMean.Text = "Mean: NA"
        '
        'btnSkip
        '
        Me.btnSkip.Location = New System.Drawing.Point(175, 88)
        Me.btnSkip.Name = "btnSkip"
        Me.btnSkip.Size = New System.Drawing.Size(142, 23)
        Me.btnSkip.TabIndex = 6
        Me.btnSkip.Text = "Skip"
        Me.btnSkip.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 64)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(78, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Select Column:"
        '
        'btnGenerateHistogram
        '
        Me.btnGenerateHistogram.Location = New System.Drawing.Point(229, 59)
        Me.btnGenerateHistogram.Name = "btnGenerateHistogram"
        Me.btnGenerateHistogram.Size = New System.Drawing.Size(75, 23)
        Me.btnGenerateHistogram.TabIndex = 8
        Me.btnGenerateHistogram.Text = "Generate Histogram"
        Me.ToolTip1.SetToolTip(Me.btnGenerateHistogram, "Generate a histogram using R")
        Me.btnGenerateHistogram.UseVisualStyleBackColor = True
        '
        'FormCalculateStatistics
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(329, 163)
        Me.Controls.Add(Me.btnGenerateHistogram)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnSkip)
        Me.Controls.Add(Me.lblMean)
        Me.Controls.Add(Me.lblMax)
        Me.Controls.Add(Me.lblMin)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnSetAsTreshold)
        Me.Controls.Add(Me.ComboBoxColumns)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "FormCalculateStatistics"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ComboBoxColumns As System.Windows.Forms.ComboBox
    Friend WithEvents btnSetAsTreshold As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblMin As System.Windows.Forms.Label
    Friend WithEvents lblMax As System.Windows.Forms.Label
    Friend WithEvents lblMean As System.Windows.Forms.Label
    Friend WithEvents btnSkip As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnGenerateHistogram As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
End Class
