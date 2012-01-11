Imports MySql.Data.MySqlClient
Imports STATCONNECTORCLNTLib
Imports StatConnectorCommonLib
Imports STATCONNECTORSRVLib

Public Class FormCalculateStatistics
    Public listFeatures As New List(Of String)
    Public connectionString As String
    Dim currFeature As Integer = 0
    Dim min, max, mean
    Dim cn As MySqlConnection, cmd As MySqlCommand, dr As MySqlDataReader, cmd1 As MySqlCommand, dr1 As MySqlDataReader


    Private Sub OpenDatabase()
        If IsNothing(cn) Then
            cn = New MySqlConnection(ConnectionString) : cn.Open()
        End If
        If cn.State = ConnectionState.Closed Then
            cn = New MySqlConnection(ConnectionString) : cn.Open()
        End If
    End Sub

    Public Sub loadFeatureColumns()
        Dim currIndex As Integer = 0
        If ComboBoxColumns.SelectedIndex <> -1 Then
            currIndex = ComboBoxColumns.SelectedIndex
        End If
        OpenDatabase()
        ComboBoxColumns.Items.Clear()
        ComboBoxColumns.Text = ""
        'gets the column names and loads them into the combo box
        cmd = New MySqlCommand("SHOW COLUMNS FROM " & listFeatures(currFeature), cn)
        dr = cmd.ExecuteReader()
        While dr.Read()
            ComboBoxColumns.Items.Add(dr(0))
        End While
        cn.Close() : dr.Close()
        ComboBoxColumns.SelectedIndex = currIndex
    End Sub

    Private Sub FormCalculateStatistics_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
       
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetAsTreshold.Click
        If ComboBoxColumns.SelectedItem <> "" Then
            OpenDatabase()
            cmd = New MySqlCommand("UPDATE genomerunner SET thresholdtype = '" & ComboBoxColumns.Text & "',thresholdmin='" & min & "',thresholdmax='" & max & "',thresholdmean='" & mean & "' WHERE featuretable='" & listFeatures(currFeature) & "';", cn)
            cmd.ExecuteNonQuery()
            cn.Close() : cmd.Dispose()
            If currFeature < listFeatures.Count - 1 Then
                currFeature += 1
                loadFeatureColumns()
                Me.Text = listFeatures(currFeature)
            Else
                Me.Close()
            End If
        End If
    End Sub

    Private Sub ComboBoxColumns_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBoxColumns.SelectedIndexChanged
        'calculates the stastical values and loads them into the labels
        If ComboBoxColumns.Text <> "" Then
            OpenDatabase()
            cmd = New MySqlCommand("SELECT MIN(" & ComboBoxColumns.Text & ")" & "FROM  " & listFeatures(currFeature), cn)
            dr = cmd.ExecuteReader()
            dr.Read()
            min = dr(0)
            lblMin.Text = "Minimum: " & min
            cn.Close() : dr.Close()
            OpenDatabase()
            cmd = New MySqlCommand("SELECT MAX(" & ComboBoxColumns.Text & ")" & "FROM  " & listFeatures(currFeature), cn)
            dr = cmd.ExecuteReader()
            dr.Read()
            max = dr(0)
            lblMax.Text = "Maximum: " & max
            cn.Close() : dr.Close()
            OpenDatabase()
            cmd = New MySqlCommand("SELECT AVG(" & ComboBoxColumns.Text & ")" & "FROM  " & listFeatures(currFeature), cn)
            dr = cmd.ExecuteReader()
            dr.Read()
            mean = dr(0)
            lblMean.Text = "Mean: " & mean
            cn.Close() : dr.Close()
            OpenDatabase()
        End If
    End Sub

    Private Sub btnSkip_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSkip.Click
        If currFeature < listFeatures.Count - 1 Then
            currFeature += 1
            loadFeatureColumns()
            Me.Text = listFeatures(currFeature)
        Else
            Me.Close()
        End If
    End Sub

    Private Sub btnGenerateHistogram_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerateHistogram.Click
        'If ComboBoxColumns.SelectedItem <> Nothing Then
        '    Dim columnValues As New List(Of Object)
        '    OpenDatabase()

        '    'gets all of the column values for the selected column
        '    cmd = New MySqlCommand("SELECT " & ComboBoxColumns.SelectedItem & " FROM " & listFeatures(currFeature), cn)
        '    dr = cmd.ExecuteReader()
        '    While dr.Read()
        '            columnValues.Add(dr(0))
        '    End While
        '    cn.Close() : dr.Close()
        '    'http://www.codeproject.com/KB/cs/RtoCSharp.aspx
        '    Dim sc1 As StatConnector = New STATCONNECTORSRVLib.StatConnector
        '    Dim n As Integer = 20, o1 As Object

        '    sc1.Init("R")
        '        Dim someerror As String = sc1.GetErrorText()
        '    sc1.SetSymbol("values", columnValues)
        '    sc1.Evaluate("columnvalues<-norm(v)")
        '    sc1.EvaluateNoReturn("hist(columnvalues")
        '    o1 = sc1.GetSymbol("x1")
        '    MsgBox("OK")
        'Else
        '    MessageBox.Show("Please select a column for which to produce a histogram")
        'End If
    End Sub
End Class