Imports MySql.Data.MySqlClient
Imports System.IO


''' <summary>
''' this program generates the table for the exons by reading off the comma deliminated exon fields from the KnownGene table and splitting them up and entering them into a new table called Exons
''' </summary>
''' <remarks></remarks>
Public Class FormExonGenerator
    Public connectionString As String
    Public listFeatures As New List(Of String)
    Dim currFeature As Integer = 0 'stores the current feature index
    Dim cn As MySqlConnection, cmd As MySqlCommand, dr As MySqlDataReader, cmd1 As MySqlCommand, dr1 As MySqlDataReader
    Dim exons As New List(Of Exon)


    Private Sub OpenDatabase()
        If IsNothing(cn) Then
            cn = New MySqlConnection(ConnectionString) : cn.Open()
        End If
        If cn.State = ConnectionState.Closed Then
            cn = New MySqlConnection(ConnectionString) : cn.Open()
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
        OpenDatabase()
        exons.Clear() 'clears the exons of previous data
        cmd = New MySqlCommand("SELECT name,chrom,strand,txStart,txEnd," & ComboBoxExonStart.Text & "," & ComboBoxExonEnd.Text & " FROM " & listFeatures(currFeature), cn)
        dr = cmd.ExecuteReader()
        If dr.HasRows Then
            While dr.Read()
                Dim nexon As New Exon
                nexon.name = dr(0)
                nexon.chrom = dr(1)
                nexon.strand = dr(2)
                nexon.txStart = dr(3)
                nexon.txEnd = dr(4)
                nexon.exonStart = dr(5)
                nexon.exonEnd = dr(6)
                exons.Add(nexon)
            End While
        End If
        cn.Close() : cmd.Dispose()
        btnRun.Enabled = False
        lblProgress.Text = "Preparing exon data" : Application.DoEvents()
        createNewExonTableStructure() 'splits up the exon end and start coordinates and orginizes them into lists
        lblProgress.Text = "Creating Exon table and populating with data" : Application.DoEvents()
        CreateExonTable()
        btnRun.Enabled = True
        MessageBox.Show("Exons tables added successfully")
        If currFeature < listFeatures.Count - 1 Then
            currFeature += 1
            loadFeatureColumns()
            Me.Text = listFeatures(currFeature)
        Else
            Me.Close()
        End If
    End Sub


    'splits up the exon end and start coordinates and orginizes them into lists
    Private Sub createNewExonTableStructure()
        ProgressBar1.Maximum = exons.Count
        ProgressBar1.Value = 0
        For Each currExon In exons
            'initilizes the lists
            currExon.listExonStart = New List(Of Integer)
            currExon.listExonEnd = New List(Of Integer)
            currExon.listName = New List(Of String)
            currExon.listChrom = New List(Of String)
            currExon.listStrand = New List(Of String)
            currExon.listTxEnd = New List(Of Integer)
            currExon.listTxStart = New List(Of Integer)


            Dim exoncoordinates As String()
            'populates the exonstarts list
            exoncoordinates = Split(currExon.exonStart, ",")
            'converts the array items from a integer to a string and adds them to the array
            For x As Integer = 0 To exoncoordinates.Count - 1
                If IsNumeric(exoncoordinates(x)) = True Then : currExon.listExonStart.Add((CInt(exoncoordinates(x)))) : End If
            Next

            exoncoordinates = Split(currExon.exonEnd, ",")
            'converts the array items from a integer to a string and adds them to the array as well as populates the name, txstart, and txend array
            For x As Integer = 0 To exoncoordinates.Count - 1
                If IsNumeric(exoncoordinates(x)) = True Then
                    currExon.listExonEnd.Add((CInt(exoncoordinates(x))))
                    'currExon.listName.Add(ConvertGeneName(currExon.name)) 'inputs the genename into the ConvertGeneName which returns the converted gene name from the database SLOW
                    currExon.listName.Add(currExon.name)
                    currExon.listChrom.Add(currExon.chrom)
                    currExon.listStrand.Add(currExon.strand)
                    currExon.listTxStart.Add(currExon.txStart)
                    currExon.listTxEnd.Add(currExon.txEnd)
                End If
            Next
            ProgressBar1.Value += 1
        Next
    End Sub

    'populates the combo boxes with the column names for the current feature
    Public Sub loadFeatureColumns()
        OpenDatabase()
        ComboBoxExonStart.Items.Clear()
        ComboBoxExonStart.Text = ""
        ComboBoxExonEnd.Items.Clear()
        ComboBoxExonEnd.Text = ""
        'gets the column names and loads them into the combo box
        cmd = New MySqlCommand("SHOW COLUMNS FROM " & listFeatures(currFeature), cn)
        dr = cmd.ExecuteReader()
        While dr.Read()
            ComboBoxExonStart.Items.Add(dr(0))
            ComboBoxExonEnd.Items.Add(dr(0))
        End While
        cn.Close() : dr.Close()
    End Sub

    'creates the table and populates it with the new data
    Private Sub CreateExonTable()
        ProgressBar1.Maximum = exons.Count
        ProgressBar1.Value = 0
        Dim filepath = Application.StartupPath & "\ExonData.txt"
        'writes the exon data to a .txt file so that it can quickly be imported into that database
        lblProgress.Text = "Writting exon data to text file for quick import into database"
        Using writer As New StreamWriter(filepath, False)
            For Each currexon In exons
                For x As Integer = 0 To currexon.listExonStart.Count - 1 Step +1
                    writer.WriteLine(currexon.listName(x) & vbTab & currexon.listChrom(x) & vbTab & currexon.listStrand(x) & vbTab & currexon.listTxStart(x) & vbTab & currexon.listTxEnd(x) & vbTab & currexon.listExonStart(x) & vbTab & currexon.listExonEnd(x))
                Next
                writer.Flush()
            Next
        End Using

        'creates a new table
        lblProgress.Text = "Creating new table called " & listFeatures(currFeature) & "Exons" : Application.DoEvents()
        OpenDatabase()
        cmd = New MySqlCommand("DROP TABLE IF EXISTS " & listFeatures(currFeature) & "Exons; CREATE  TABLE `" & listFeatures(currFeature) & "Exons` (`name` VARCHAR(50) NULL , `chrom` VARCHAR(45) NULL, `strand` VARCHAR(45) NULL ,  `txStart` INT NULL ,  `txEnd` INT NULL ,  `exonStart` INT NULL ,  `exonEnd` INT NULL); ", cn)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
        cn.Close() : dr.Close()

        'reads the data in the text file into the new exon table
        Dim query As String = ""
        lblProgress.Text = "Creating new table called " & listFeatures(currFeature) & "Exon" : Application.DoEvents()
        Try
            'clears all data that might have existed in the Exon table
            OpenDatabase()
            query = "TRUNCATE TABLE " & listFeatures(currFeature) & "Exons;"
            cmd = New MySqlCommand(query, cn)
            cmd.ExecuteNonQuery()
            cmd.Dispose() : cn.Close() : dr.Close()
            OpenDatabase()
            'loads the new data into the exon table
            Dim filePathFeatureToAddUNIX As String = filepath.Replace("\", "/") 'modifies the filepath to conform to UNIX
            query = "LOAD DATA LOCAL INFILE " & "'" & filePathFeatureToAddUNIX & "' INTO TABLE " & listFeatures(currFeature) & "Exons;"
            cmd = New MySqlCommand(query, cn)
            cmd.ExecuteNonQuery()
            cmd.Dispose() : cn.Close() : dr.Close()
        Catch
            Dim form As New FormQuery
            form.txtQuery.Text = query
            form.ShowDialog()
        End Try
    End Sub


    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSkip.Click
        If currFeature < listFeatures.Count - 1 Then
            currFeature += 1
            loadFeatureColumns()
            Me.Text = listFeatures(currFeature)
        Else
            Me.Close()
        End If
    End Sub
End Class

Public Class Exon
    Public name As String
    Public chrom As String
    Public strand As String
    Public txStart As Integer
    Public txEnd As Integer
    Public exonStart As String
    Public exonEnd As String

    'this is where the results of spliting the ExonStart and Exon End 
    Public listExonStart As List(Of Integer)
    Public listExonEnd As List(Of Integer)
    Public listName As List(Of String)
    Public listStrand As List(Of String)
    Public listChrom As List(Of String)
    Public listTxStart As List(Of Integer)
    Public listTxEnd As List(Of Integer)
End Class
