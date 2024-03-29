﻿Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Net
Imports alglib.hqrnd

Public Class FormMain

    Dim listGRFeatures As List(Of Feature) 'used to store feature values that are retrieved from the database
    Dim cn As MySqlConnection, cmd As MySqlCommand, dr As MySqlDataReader, cmd1 As MySqlCommand, dr1 As MySqlDataReader
    Dim featuresSelected As New ArrayList 'stores the features that are selected in the list box
    Dim ftpHost As String, ftpUser As String = "anonymous", ftpPassword As String = "" 'ftpHost will be set in OpenDatabase() using field from user
    Dim uName As String = "", uServer As String = "", uPassword As String = "", uDatabase As String = ""
    Dim arrayFeaturesToAdd As New ArrayList, arrayFeaturesAdded As New ArrayList, arrayFeaturesToRemove As New ArrayList, arrayFeaturesEmpty As New ArrayList, arrayFeaturesToUpdate As New ArrayList, arrayFeaturesNearlyUpToDate As New ArrayList, arrayFeaturesNotAddedSuccessfully As New ArrayList 'array of features to be added
    Dim arrayFeaturesCompleted As New ArrayList
    Dim fileName As String, numOfFeaturesToAdd As Integer = 0, progress As Integer = 1 'stores the value of the progress bar
    Dim ConnectionString As String, binary As String
    Dim Features As New List(Of Feature)
    Dim RandomFeatures As New List(Of Feature)
    Dim backgroundFeatures As New List(Of Feature)
    Dim featureTables As New List(Of String)
    Dim bkgNum As Integer
    Private DataDownloadPath As String = ""
    Private PopulateErrors As New List(Of String)

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        DataDownloadPath = GetSetting("Genome Feature Table Creator", "Data", "DataDownloadPath", AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\")
        lblDataDownloadPath.Text = DataDownloadPath
    End Sub

    'This method ensures that FeatureTable column doesn't end up with illegal chars in it.
    Private Function RemoveIllegalChars(ByRef StringToTrim As String) As String
        Dim illegalChars As Char() = "!@#$%^&*(){}""+'<>?/\:.-" & vbLf.ToCharArray()
        Dim sb As New System.Text.StringBuilder
        For Each ch As Char In StringToTrim
            If Array.IndexOf(illegalChars, ch) = -1 Then
                sb.Append(ch)
            End If
        Next
        Return sb.ToString
    End Function

    'opens a connection to the database; uses values supplied by the user
    Private Sub OpenDatabase()
        Dim uName As String = txtUser.Text
        Dim uServer As String = txtHost.Text
        Dim uPassword As String = txtPassword.Text
        Dim uDatabase As String = txtDatabase.Text
        Dim uUcscDatabaseName As String = txtUcscdb.Text

        ' If uName <> "" And uPassword <> "" And uServer <> "" And uDatabase <> "" Then
        ConnectionString = "Server=" & uServer & ";Database=" & uDatabase & ";User ID=" & uName & ";Password=" & uPassword & ";Connection Timeout=6000;default command timeout=6000" 'uses the values provided by user to create constring. TODO add error checking
        ftpHost = "ftp://hgdownload.cse.ucsc.edu/goldenPath/" & uUcscDatabaseName & "/database/"
        ' Else
        ' ConnectionString = "Server=VM-Wren-01;Database=hg18;User ID=Genome;Password=Runner"
        ' End If
        'If IsNothing(cn) Then

        If IsNothing(cn) Then
            cn = New MySqlConnection(ConnectionString) : cn.Open()
        End If
        If cn.State = ConnectionState.Closed Then
            cn = New MySqlConnection(ConnectionString) : cn.Open()
        End If
    End Sub

    'connects to the db
    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        btnExonTable.Enabled = True
        listFeaturesToAdd.Items.Clear() 'clears the list
        Try
            OpenDatabase()
            'RP: Commented out this next line b/c it should use DownloadFeatureList() first;
            '    otherwise it's just using a previously saved file that doesn't necessarily
            '    correspond to the currently selected UCSC database.
            '    For now this is handled when the user clicks "Update Features List".
            'GetFeaturesAvailableList() 'gets list of features that can be added
            If TableExists("genomerunner") Then
                GetAddedFeatures() 'loads the table names of features added to list
                'GetEmptyFeatures() 'checks for features in the db that are empty
                'syncGenomeTableToDatabase() 'checks for features that have been added to the genomerunner table but not actually added to the database
            Else
                MessageBox.Show("Connection successfully established, but selected database has no genomerunner table." & vbCrLf & "Please, load genomerunner table first.")
            End If
        Catch
            MessageBox.Show("Please check connection settings and try again")
            'Now close connection if for some reason it was left open.
            If Not IsNothing(cn) Then
                If cn.State = 1 Then cn.Close()
            End If
        End Try
    End Sub

    Private Function TableExists(ByVal tableName As String) As Boolean
        Dim exists As Boolean = False
        cmd = New MySqlCommand("SHOW TABLES LIKE '%" & tableName & "%'", cn)
        dr = cmd.ExecuteReader
        If dr.HasRows Then exists = True
        cmd.Dispose() : dr.Close()
        Return exists
    End Function

    'get features already added to the genomerunnertable
    Private Sub GetAddedFeatures()
        OpenDatabase()
        arrayFeaturesAdded.Clear()
        arrayFeaturesCompleted.Clear()
        listFeaturesToAdd.Items.Clear()
        cmd = New MySqlCommand("SELECT featuretable,complete FROM genomerunner ORDER BY ID", cn)
        Dim dr As MySqlDataReader
        dr = cmd.ExecuteReader()
        While dr.Read()
            arrayFeaturesAdded.Add(dr(0))
            arrayFeaturesCompleted.Add(dr(1))
            listFeaturesToAdd.Items.Add(arrayFeaturesAdded(arrayFeaturesAdded.Count - 1))
        End While
        cmd.Dispose() : dr.Close()
    End Sub

    Private Sub btnMakeDatabase_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMakeDatabase.Click
        Dim listLength As Integer = listFeaturesToAdd.Items.Count - 1
        For i = 0 To listLength
            binary = Convert.ToString(arrayFeaturesCompleted(i), 2)
            If binary = "" Or binary = "0" Then binary = "00000"

            'If arrayFeaturesAdded(i) = "kgXref" Or arrayFeaturesAdded(i) = "polyaPredict" Then
            '    Debug.Print("Yes")
            'End If

            If Mid(binary, 1, 1) = "0" Then 'If no exist at the first place
                PrepareFeatureFiles(arrayFeaturesAdded(i)) 'Download it
            End If
            If Mid(binary, 2, 1) = "0" Then 'If the table is not exist
                If "1" = Mid(binary, 1, 1) Then AddTableToDatabase(arrayFeaturesAdded(i)) 'Created table. But check before if the Flag1 is set to 1, i.e. the data are downloaded
            End If
            If Mid(binary, 3, 1) = "0" Then 'If the data are not in the table
                If ("1" = Mid(binary, 1, 1) And "1" = Mid(binary, 2, 1)) Then PopulateDatabse(arrayFeaturesAdded(i)) 'Populate the table. But check if Flag1/2 are set to 1, i.e. the data are downloaded and the table is created
            End If
            If Mid(binary, 4, 1) = "0" Then
                CreateExonTable(arrayFeaturesAdded(i)) 'Check if exon table should be created, and create one
            End If
            If Mid(binary, 5, 1) = "0" Then
                DeleteFeatureFiles(arrayFeaturesAdded(i))
                binary = "00000"
            End If
            CheckIfErrorsFixed(arrayFeaturesAdded(i))
            OpenDatabase() 'Write the status of all operations
            cmd = New MySqlCommand("UPDATE genomerunner SET complete=" & Convert.ToInt32(binary, 2) & " WHERE featuretable='" & arrayFeaturesAdded(i) & "';", cn)
            cmd.ExecuteNonQuery() : cmd.Dispose()

            'Select Case arrayFeaturesCompleted(i)
            '    Case 0 'Table not exist
            '        binary = "00000"
            '        PrepareFeatureFiles(arrayFeaturesAdded(i))
            '        If "1" = Mid(binary, 1, 1) Then AddTableToDatabase(arrayFeaturesAdded(i)) 'Continue if .sql and .txt.gz files were downloaded
            '        If "1" = Mid(binary, 2, 1) Then PopulateDatabse(arrayFeaturesAdded(i)) 'Continue if the table was created successfully
            '        CreateExonTable(arrayFeaturesAdded(i)) 'Check if exon table should be created, and create one
            '        CheckIfErrorsFixed(arrayFeaturesAdded(i)) 'If errors were fixed by manually executing faulty queries, change the flags
            '        Mid(binary, 5, 1) = "1" 'Last flag indicates the table was just updated
            '        OpenDatabase() 'Write the status of all operations
            '        cmd = New MySqlCommand("UPDATE genomerunner SET complete=" & Convert.ToInt32(binary, 2) & " WHERE featuretable='" & arrayFeaturesAdded(i) & "';", cn)
            '        cmd.ExecuteNonQuery() : cmd.Dispose()
            '    Case 1
            '        PrepareFeatureFiles(arrayFeaturesAdded(i))
            '        PopulateDatabse(arrayFeaturesAdded(i))
            '        CreateExonTable(arrayFeaturesAdded(i))
            '        OpenDatabase()
            '        cmd = New MySqlCommand("UPDATE genomerunner SET complete=9 WHERE featuretable='" & arrayFeaturesAdded(i) & "';", cn)
            '        cmd.ExecuteNonQuery() : cmd.Dispose()
            '    Case 8
            '    Case 30 '11110(BIN) Data need update
            '        binary = Convert.ToString(arrayFeaturesCompleted(i), 2)
            '    Case 31
            'End Select
        Next
        'DatabaseLastUpdated()
        GetAddedFeatures()
    End Sub

    'changes the color of the list items
    Private Sub listFeaturesToAdd_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles listFeaturesToAdd.DrawItem
        '[1/0]-(Data&sql exist) [1/0]-(Table created) [1/0]-(Data loaded) [1/0]-(Exon table created, if needed) [1/0]-(Table&data up-to-date)
        e.DrawBackground()
        Dim myBrush As Brush = Brushes.Red
        Select Case arrayFeaturesCompleted(e.Index)
            Case 0 '00000(BIN) Table not exist
                myBrush = Brushes.Red
                'Case 1 'Table structure exist, but no data
                '    myBrush = Brushes.Orange
                'Case 2 'Data error
                '    myBrush = Brushes.DimGray
                'Case 3 'Table error
                '    myBrush = Brushes.DimGray
                'Case 8 'Table exist, data needs to be updated
                '    myBrush = Brushes.Green
            Case 30 '11110(BIN) Data need update
                myBrush = Brushes.Green
            Case 31 '11111(BIN) Table & data up-to-date
                myBrush = Brushes.Black
            Case Else
                myBrush = Brushes.AliceBlue
        End Select
        e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)
        e.DrawFocusRectangle()
    End Sub

    'downloads and decompresses the feature 
    Private Sub PrepareFeatureFiles(ByVal feature As String)
        Debug.Print("PrepareFeatureFiles " & feature)
        Try
            If File.Exists(DataDownloadPath & feature & ".sql") = False Then
                lblProgress.Text = "Downloading " & feature & ".sql" : Application.DoEvents()
                My.Computer.Network.DownloadFile(ftpHost & feature & ".sql", DataDownloadPath & feature & ".sql") 'downloads the .sql feature file
                System.Threading.Thread.Sleep(1000)
            End If
            If File.Exists(DataDownloadPath & feature & ".txt.gz") = False Then
                lblProgress.Text = "Downloading " & feature & ".txt.gz" : Application.DoEvents()
                My.Computer.Network.DownloadFile(ftpHost & feature & ".txt.gz", DataDownloadPath & feature & ".txt.gz") 'downloads the .txt.gz feature file
                System.Threading.Thread.Sleep(15000)
            End If
            Mid(binary, 1, 1) = "1"
        Catch
            MessageBox.Show("Unable to download files for " & feature)
            Mid(binary, 1, 1) = "0"
        End Try
    End Sub

    Private Sub AddTableToDatabase(ByVal feature As String)
        Debug.Print("AddTableToDatabase " & feature)
        Dim filePathFeaturesToAdd = DataDownloadPath
        If File.Exists(filePathFeaturesToAdd & feature & ".sql") = True Then 'Check if .sql file exist
            Dim filestream As String = File.ReadAllText(filePathFeaturesToAdd & feature & ".sql")
            Dim query As String = vbNullString
            OpenDatabase()
            Dim querySuccessful As Boolean = False
            Dim queryDidFail As Boolean = False
            query = filestream.Replace("longblob", "longtext") 'sets the initial query, replaces "longblob" with "longtext"
            query = query.Replace("TYPE", "ENGINE")
            query = query.Replace("SET character_set_client = @saved_cs_client;", " ")
            query = query.Replace("SET @saved_cs_client     = @@character_set_client;", " ")
            query = query.Replace("SET character_set_client = utf8;", " ")
            query = query.Insert(0, "DROP TABLE IF EXISTS " & feature & "; ")
            While querySuccessful = False 'continues till the query is successful
                OpenDatabase()
                Try
                    cmd = New MySqlCommand(query, cn) 'executes the query
                    cmd.ExecuteNonQuery()
                    lblProgress.Text = "Creating table " & feature
                    querySuccessful = True 'sets query status as successful. causes the loop to end
                    If queryDidFail = True Then
                        Dim response As Boolean = MsgBox("Query successful, do  you want to overwrite the old query file?", vbYesNo) 'asks the user if they want to overwrite the old query with the new
                        If response = True Then
                            File.WriteAllText(filePathFeaturesToAdd & feature & ".sql", query) 'replaces the query file with a new query
                        End If
                    End If
                    Mid(binary, 2, 1) = "1"
                Catch ex As Exception
                    Dim form As New FormMysqlQueryEditor() 'creates a form for the user to change the query
                    form.originQuery = filestream.Replace("longblob", "longtext")
                    form.LabelStatus.Text = "There was an error executing the query.  Please review the query for proper formatting and resubmit."
                    form.LabelErrorMessage.Text = "Error Message: " & ex.Message
                    form.ShowDialog()
                    If form.skipQuery = True Then
                        Mid(binary, 2, 1) = "0" : Exit While
                    End If
                    query = form.editedQuery
                    queryDidFail = True
                End Try
            End While
        End If
    End Sub

    Private Sub PopulateDatabse(ByVal feature As String)
        Debug.Print("PopulateDatabase " & feature)
        OpenDatabase()
        'decompresses this feature's .gz file to a .txt file
        If File.Exists(DataDownloadPath & feature & ".txt") = False Then
            Using outfile As FileStream = File.Create(DataDownloadPath & feature & ".txt")
                Using infile As FileStream = File.OpenRead(DataDownloadPath & feature & ".txt.gz")
                    Using Decompress As System.IO.Compression.GZipStream = New System.IO.Compression.GZipStream(infile, Compression.CompressionMode.Decompress)
                        Decompress.CopyTo(outfile)
                    End Using
                End Using
            End Using
        End If

        'Need to query extra data if this table's genomerunner entry has 'Threshold' as it's QueryType
        Dim thresholdType As String = ""
        Dim thresholdQuery As String = ""
        cmd = New MySqlCommand("SELECT QueryType, ThresholdType FROM genomerunner WHERE FeatureTable = '" & feature & "';", cn)
        dr = cmd.ExecuteReader
        While dr.Read
            If dr(0) = "Threshold" Then
                thresholdType = dr(1)
                thresholdQuery = ", thresholdMin = (SELECT MIN(" & thresholdType & ") from " & feature & _
                                 "), thresholdMax = (SELECT MAX(" & thresholdType & ") from " & feature & _
                                 "), thresholdMean = (SELECT AVG(" & thresholdType & ") from " & feature & ")"
            End If
        End While
        dr.Close() : cmd.Dispose()

        'Populate
        Dim filePathFeaturesToAdd = DataDownloadPath
        Dim filePathFeatureToAddUNIX As String = filePathFeaturesToAdd.Replace("\", "/") 'modifies the filepath to conform to UNIX 
        Dim query As String = "TRUNCATE TABLE " & feature & ";LOAD DATA LOCAL INFILE " & "'" & filePathFeatureToAddUNIX & feature & ".txt' INTO TABLE " & feature & ";" & _
                              "UPDATE genomerunner SET count = (SELECT COUNT(*) FROM " & feature & ")" & thresholdQuery & " WHERE FeatureTable = '" & feature & "';"
        Try
            cmd = New MySqlCommand(query, cn) 'reads the .txt file and inserts the data into the created table
            cmd.ExecuteNonQuery()
            lblProgress.Text = "Loading data " & feature : Application.DoEvents()
            Mid(binary, 3, 1) = "1" : Mid(binary, 5, 1) = "1"
            OpenDatabase()
            cmd = New MySqlCommand("UPDATE genomerunner SET time='" & DateTime.Now.ToString("MM-dd-yyyy") & "' WHERE featuretable='" & feature & "';", cn)
            cmd.ExecuteNonQuery() : cmd.Dispose()
            Debug.Print("PopulateDatabase " & feature)
        Catch ex As Exception
            Dim form As New FormQuery
            form.txtQuery.Text = query
            form.ShowDialog()
            Mid(binary, 3, 1) = "0"
        End Try
        dr.Close() : cmd.Dispose()
        'Now delete unzipped .txt file to save space.
        File.Delete(filePathFeaturesToAdd & feature & ".txt")
        If feature = "rmsk" Then Fix_rmsk_table() 'If the feature just loaded is rmsk table, fix column names
        'Update time stamp when the data were loaded
    End Sub

    Private Sub CreateExonTable(ByVal feature As String)
        Dim exons As List(Of Exon)
        OpenDatabase()
        cmd = New MySqlCommand("SELECT QueryType FROM genomerunner WHERE FeatureTable = '" & feature & "';", cn)
        dr = cmd.ExecuteReader
        dr.Read()
        Dim queryType = dr(0)
        dr.Close() : cmd.Dispose()
        'Creating exon table is only necessary if queryType = "Gene"
        If queryType = "Gene" Then
            exons = GetExons(feature)
            CreateTableFromExons(feature, exons)
        Else
            Mid(binary, 4, 1) = "1"
        End If
    End Sub

    Private Function GetExons(ByVal feature As String) As List(Of Exon)
        Dim exons As New List(Of Exon)
        cmd = New MySqlCommand("SELECT name,chrom,strand,txStart,txEnd,exonStarts,exonEnds FROM " & feature, cn)
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
                'initilizes the lists
                nexon.listExonStart = New List(Of Integer)
                nexon.listExonEnd = New List(Of Integer)
                nexon.listName = New List(Of String)
                nexon.listChrom = New List(Of String)
                nexon.listStrand = New List(Of String)
                nexon.listTxEnd = New List(Of Integer)
                nexon.listTxStart = New List(Of Integer)
                exons.Add(nexon)
            End While
        End If
        cn.Close() : cmd.Dispose()

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Now split up data in exons
        For Each currExon In exons

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
        Next
        Return exons
    End Function

    Private Sub CreateTableFromExons(ByVal feature As String, ByVal exons As List(Of Exon))
        ProgressBar1.Maximum = exons.Count
        ProgressBar1.Value = 0
        Dim filepath = Application.StartupPath & "\ExonData.txt"
        'writes the exon data to a .txt file so that it can quickly be imported into that database
        Using writer As New StreamWriter(filepath, False)
            For Each currexon In exons
                For x As Integer = 0 To currexon.listExonStart.Count - 1 Step +1
                    writer.WriteLine(currexon.listName(x) & vbTab & currexon.listChrom(x) & vbTab & currexon.listStrand(x) & vbTab & currexon.listTxStart(x) & vbTab & currexon.listTxEnd(x) & vbTab & currexon.listExonStart(x) & vbTab & currexon.listExonEnd(x))
                Next
                writer.Flush()
            Next
        End Using

        'creates a new table
        OpenDatabase()
        cmd = New MySqlCommand("DROP TABLE IF EXISTS " & feature & "Exons; CREATE  TABLE `" & feature & "Exons` (`name` VARCHAR(50) NULL , `chrom` VARCHAR(45) NULL, `strand` VARCHAR(45) NULL ,  `txStart` INT NULL ,  `txEnd` INT NULL ,  `exonStart` INT NULL ,  `exonEnd` INT NULL); ", cn)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
        cn.Close() : dr.Close()

        'reads the data in the text file into the new exon table
        Dim query As String = ""
        lblProgress.Text = "Load exon data " & feature : Application.DoEvents()
        Try
            'clears all data that might have existed in the Exon table
            OpenDatabase()
            query = "TRUNCATE TABLE " & feature & "Exons;"
            cmd = New MySqlCommand(query, cn)
            cmd.ExecuteNonQuery()
            cmd.Dispose() : dr.Close()
            OpenDatabase()
            'loads the new data into the exon table
            Dim filePathFeatureToAddUNIX As String = filepath.Replace("\", "/") 'modifies the filepath to conform to UNIX
            query = "LOAD DATA LOCAL INFILE " & "'" & filePathFeatureToAddUNIX & "' INTO TABLE " & feature & "Exons;"
            cmd = New MySqlCommand(query, cn)
            cmd.ExecuteNonQuery()
            cmd.Dispose() : dr.Close()
            Mid(binary, 4, 1) = "1"
            OpenDatabase()
            cmd = New MySqlCommand("UPDATE genomerunner SET time='" & DateTime.Now.ToString("MM-dd-yyyy") & "' WHERE featuretable='" & feature & "Exons';", cn)
            cmd.ExecuteNonQuery() : cmd.Dispose()
        Catch
            Dim form As New FormQuery
            form.txtQuery.Text = query
            form.ShowDialog()
            Mid(binary, 4, 1) = "0"
        End Try
        Debug.Print("CreateTableFromExons " & feature)
    End Sub

    Private Sub CheckIfErrorsFixed(ByVal feature As String)
        Debug.Print("CheckIfErrorsFixed " & feature)
        If "0" = Mid(binary, 3, 1) Then 'If the data was marked as not loaded, check if it has been done manually
            Try
                OpenDatabase()
                cmd = New MySqlCommand("SELECT * FROM " & feature & " LIMIT 1") 'Is there anything in the database?
                dr = cmd.ExecuteReader
                If dr.HasRows Then Mid(binary, 3, 1) = "1" 'If yes, set the flag as completed
                dr.Close() : cmd.Dispose()
            Catch ex As Exception
                Mid(binary, 3, 1) = "0"
            End Try
        End If
        If "0" = Mid(binary, 4, 1) Then 'If the EXON data was marked as not loaded, check if it has been done manually
            Try
                OpenDatabase()
                cmd = New MySqlCommand("SELECT * FROM " & feature & "exons LIMIT 1") 'Is there anything in the database?
                dr = cmd.ExecuteReader
                If dr.HasRows Then Mid(binary, 4, 1) = "1" 'If yes, set the flag as completed
                dr.Close() : cmd.Dispose()
            Catch ex As Exception
                Mid(binary, 4, 1) = "0"
            End Try
        End If

        Dim ExonTable As Boolean = False
        OpenDatabase()
        cmd = New MySqlCommand("SELECT QueryType FROM genomerunner WHERE FeatureTable='" & feature & "';", cn)
        dr = cmd.ExecuteReader
        If dr.HasRows Then
            dr.Read()
            If dr(0) = "Gene" Then ExonTable = True
        End If
        dr.Close() : cmd.Dispose()
        If ExonTable Then
            cmd = New MySqlCommand("SELECT * FROM " & feature & "Exons LIMIT 2;", cn)
            Try
                dr = cmd.ExecuteReader
                If dr.HasRows Then Exit Try
            Catch ex As Exception
                CreateExonTable(feature)
            End Try
            dr.Close() : cmd.Dispose()
        End If

        If Mid(binary, 1, 4) = "1111" Then
            OpenDatabase()
            cmd = New MySqlCommand("SELECT count FROM genomerunner WHERE FeatureTable='" & feature & "';", cn)
            dr = cmd.ExecuteReader
            Dim GRcount As Integer = 0
            If dr.HasRows Then
                dr.Read() : GRcount = dr(0)
            End If
            dr.Close() : cmd.Dispose()
            OpenDatabase()
            cmd = New MySqlCommand("SELECT COUNT(*) FROM " & feature & ";", cn)
            dr = cmd.ExecuteReader
            If dr.HasRows Then
                dr.Read() : If dr(0) = GRcount Then
                    Mid(binary, 5, 1) = "1"
                Else
                    Mid(binary, 5, 1) = "0"
                End If
            End If
            dr.Close() : cmd.Dispose()
        End If
    End Sub

    Private Sub Fix_rmsk_table()
        OpenDatabase()
        If TableExists("rmsk") Then
            cmd = New MySqlCommand("ALTER TABLE rmsk CHANGE COLUMN `genoName` `chrom` VARCHAR(255) NOT NULL DEFAULT '', CHANGE COLUMN `genoStart` `chromStart` INT(10) UNSIGNED NOT NULL DEFAULT '0', CHANGE COLUMN `genoEnd` `chromEnd` INT(10) UNSIGNED NOT NULL DEFAULT '0'", cn) ' DROP INDEX `genoName`, ADD INDEX `genoName` (`chrom`(14) ASC, `bin` ASC);", cn)
            cmd.ExecuteNonQuery() : cmd.Dispose()
        End If
    End Sub

    Private Sub DeleteFeatureFiles(ByVal feature As String)
        If File.Exists(DataDownloadPath & feature & ".sql") = True Then
            File.Delete(DataDownloadPath & feature & ".sql")
        End If
        If File.Exists(DataDownloadPath & feature & ".txt.gz") = True Then
            File.Delete(DataDownloadPath & feature & ".txt.gz")
        End If
    End Sub

    Private Sub btnLoadGRTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadGRTable.Click

        Dim button As DialogResult = MessageBox.Show("Do you want to replace completely from a file (Yes) or update (No) genomerunner table?", "Replace or update GenomeRunner table", _
                           MessageBoxButtons.YesNoCancel)
        If button = Windows.Forms.DialogResult.Yes Then
            OpenDatabase()
            cmd = New MySqlCommand("DROP TABLE IF EXISTS genomerunner;", cn)
            cmd.ExecuteNonQuery() : cmd.Dispose()
            CreateGenomeRunnerTable()
        ElseIf button = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If

        Dim OpenFD As New OpenFileDialog
        OpenFD.Title = "Select a tab delimited .txt file with list of features to be imported to genomerunner table."
        OpenFD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
        OpenFD.FileName = ""
        OpenFD.Filter = "All files|*.*|Text Files|*.txt"
        OpenFD.ShowDialog()
        fileName = OpenFD.FileName
        If fileName <> "" Then
            Debug.Print("Creating genomerunner table")
            CreateGenomeRunnerTable()   'Create new table, if the old one does not exist. If exist, nothing will be created
            OpenDatabase()
            Using reader As StreamReader = New StreamReader(fileName) 'reads the list of features
                reader.ReadLine()       'skip header row
                While Not reader.EndOfStream
                    Dim line As Array = Split(reader.ReadLine, vbTab)
                    'line could contain null values which will cause an sql error. Insert 'NULL' when this is the case.
                    For i As Integer = 0 To line.Length - 1 Step +1
                        If line(i) = "" Then line(i) = "NULL"
                    Next
                    Dim FeatureTable As String = Trim(line(1))
                    Dim FeatureName As String = Trim(line(2)).Replace("""", vbNullString)
                    'FeatureName = RemoveIllegalChars(FeatureName)

                    lblProgress.Text = "Loading genomerunner entry for " & FeatureTable : Application.DoEvents()
                    'TODO if entry already exists, get record count. If it matches file, keep all calculated values.
                    cmd = New MySqlCommand("SELECT * FROM genomerunner WHERE FeatureTable = '" & FeatureTable & "';", cn)
                    dr = cmd.ExecuteReader()
                    If dr.HasRows Then
                        'already exists; update existing entry with values from loaded file
                        cmd = New MySqlCommand("UPDATE genomerunner SET id=" & line(0) & ", FeatureName='" & FeatureName & "', QueryType='" & line(3) & "', ThresholdType='" & line(4) & "',Tier=" & line(9) & ",Category='" & line(10) & "', orderofcategory=" & line(11) & ", Name='" & line(12) & "' WHERE FeatureTable = '" & FeatureTable & "';", cn)
                    Else
                        'doesn't exist; create new entry
                        'NOTE: this old cmd updates all calculated fields (min, max, etc) as well.
                        'cmd = New MySqlCommand("INSERT INTO genomerunner (id, FeatureTable, FeatureName, QueryType, ThresholdType, ThresholdMin, ThresholdMax, ThresholdMean, ThresholdMedian, Tier, Category, orderofcategory, Name, count, min, max, mean, median, length) VALUES (" & _
                        '                       line(0) & ", '" & line(1) & "', '" & line(2) & "', '" & line(3) & "', '" & line(4) & "', " & line(5) & ", " & line(6) & ", " & line(7) & ", " & line(8) & ", " & line(9) & ", '" & line(10) & "', " & line(11) & ", '" & line(12) & "', " & line(13) & ", " & line(14) & ", " & line(15) & ", " & line(16) & ", " & line(17) & ", " & line(18) & ")", cn)
                        cmd = New MySqlCommand("INSERT INTO genomerunner (id, FeatureTable, FeatureName, QueryType, ThresholdType, Tier, Category, orderofcategory, Name) VALUES (" & _
                                           line(0) & ", '" & Trim(line(1)) & "', '" & FeatureName & "', '" & line(3) & "', '" & line(4) & "', " & line(9) & ", '" & line(10) & "', " & line(11) & ", '" & line(12) & "');", cn)
                    End If
                    dr.Close()
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()
                End While
            End Using
            lblProgress.Text = "Done" : Application.DoEvents()
        End If
        GetAddedFeatures()
    End Sub

    Private Sub CreateGenomeRunnerTable()
        OpenDatabase()
        cmd = New MySqlCommand("CREATE TABLE IF NOT EXISTS genomerunner (" & _
                                "id INT(11) NOT NULL," & _
                                "FeatureTable VARCHAR(200) PRIMARY KEY," & _
                                "FeatureName VARCHAR(300)," & _
                                "QueryType VARCHAR(45) DEFAULT 'NA'," & _
                                "ThresholdType VARCHAR(45) DEFAULT 'NA'," & _
                                "ThresholdMin FLOAT DEFAULT 0," & _
                                "ThresholdMax FLOAT DEFAULT 0," & _
                                "ThresholdMean FLOAT DEFAULT 0," & _
                                "ThresholdMedian FLOAT DEFAULT 0," & _
                                "Tier INT(11) DEFAULT 3," & _
                                "Category VARCHAR(45) DEFAULT '1000|unknown'," & _
                                "orderofcategory INT(11) DEFAULT 0," & _
                                "Name VARCHAR(45)," & _
                                "count INT(11) DEFAULT 0," & _
                                "min INT(11) DEFAULT 0," & _
                                "max INT(11) DEFAULT 0," & _
                                "mean INT(11) DEFAULT 0," & _
                                "median INT(11) DEFAULT 0," & _
                                "length BIGINT(11) UNSIGNED DEFAULT 0," & _
                                "complete INT(11) DEFAULT 0," & _
                                "time TEXT" & _
                                ");", cn)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub

    Private Sub btnCompareToUCSC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCompareToUCSC.Click
        'NOTE: This command will get row count for each table in the selected database.
        '"SELECT table_name, TABLE_ROWS FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'mm9test';"
        'UCSC db info: mysql --user=genome --host=genome-mysql.cse.ucsc.edu -A

        Dim featuresForUCSCQuery As New List(Of String)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Row count from local genomerunner
        Dim localTablesToCounts As New Hashtable 'key is FeatureTable, value is count
        OpenDatabase()
        cmd = New MySqlCommand("SELECT featureTable, count FROM genomerunner;", cn)
        dr = cmd.ExecuteReader
        While dr.Read
            localTablesToCounts.Add(dr(0), dr(1))
            lblProgress.Text = "Reading local table: " & dr(0) : Application.DoEvents()
            featuresForUCSCQuery.Add("'" & dr(0) & "'")
            'TODO asynchronous?
        End While
        cmd.Dispose()
        dr.Close()

        'establish connection with UCSC database
        Dim ucscCn As MySqlConnection, ucscCmd As MySqlCommand, ucscDr As MySqlDataReader
        ucscCn = New MySqlConnection("Server=genome-mysql.cse.ucsc.edu" & ";Database=" & txtUcscdb.Text & ";User=genomep; Password=password;Connection Timeout=6000;default command timeout=6000") : ucscCn.Open()
        ucscCmd = New MySqlCommand("SELECT table_name, TABLE_ROWS FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" & txtUcscdb.Text & "' AND table_name IN (" & Join(featuresForUCSCQuery.ToArray, ",") & ");", ucscCn)
        ucscDr = ucscCmd.ExecuteReader()

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Row count from UCSC
        Dim ucscTablesToCounts As New Hashtable 'key is table_name, value is rows in table
        'column names in the results:
        'table_name, TABLE_ROWS
        While ucscDr.Read()
            ucscTablesToCounts.Add(ucscDr(0), ucscDr(1))
            'TODO asynchronous?
            lblProgress.Text = "Reading UCSC table: " & ucscDr(0) : Application.DoEvents()
        End While
        ucscCmd.Dispose() : ucscDr.Close()

        'Now look compare results with genomerunner table.
        For Each feature In localTablesToCounts.Keys
            'If String.Compare(localTablesToCounts(feature), ucscTablesToCounts(feature), True) Then 'comparison will ignore case
            'If localTablesToCounts(feature.ToString.ToLower) <> ucscTablesToCounts(feature.ToString.ToLower) Then
            '    'BEWARE: differences in lower/upper case could be an issue here.
            '    arrayFeaturesAdded.Remove(feature)
            '    If (Math.Abs(ucscTablesToCounts(feature) - localTablesToCounts(feature)) \ ucscTablesToCounts(feature)) <= 0.1 Then
            '        arrayFeaturesNearlyUpToDate.Add(feature)
            '    Else
            '        arrayFeaturesToUpdate.Add(feature)
            '    End If
            'End If

            'If localTablesToCounts(feature.ToString.ToLower) = ucscTablesToCounts(feature.ToString.ToLower) Then
            If feature = "cytoBand" Then
                Debug.Print(feature)
            End If
            If Not (ucscTablesToCounts(feature) = 0 And localTablesToCounts(feature) = 0) Then
                'If (Math.Abs(ucscTablesToCounts(feature) - localTablesToCounts(feature)) / ucscTablesToCounts(feature)) >= 0.1 Then 'Discrepancies in 10% not counted
                If Math.Abs(ucscTablesToCounts(feature)) <> Math.Abs(localTablesToCounts(feature)) Then 'Exact match required
                    Debug.Print("Mismatch. Feature: " & feature & "; UCSC: " & ucscTablesToCounts(feature) & "; local: " & localTablesToCounts(feature))
                    OpenDatabase()
                    cmd = New MySqlCommand("SELECT complete FROM genomerunner WHERE FeatureTable='" & feature & "';", cn)
                    dr = cmd.ExecuteReader : dr.Read()
                    binary = Convert.ToString(dr(0), 2)
                    dr.Close() : cmd.Dispose()
                    If binary <> 0 Then Mid(binary, 5, 1) = "0"
                    cmd = New MySqlCommand("UPDATE genomerunner SET complete=" & Convert.ToInt32(binary, 2) & " WHERE FeatureTable='" & feature & "';", cn)
                    cmd.ExecuteNonQuery() : cmd.Dispose()
                End If
            Else
                OpenDatabase()
                cmd = New MySqlCommand("UPDATE genomerunner SET QueryType='NA', ThresholdType='NA' WHERE FeatureTable='" & feature & "';", cn)
                cmd.ExecuteNonQuery() : cmd.Dispose()
            End If
        Next
        cn.Close()
        lblProgress.Text = "Sync with UCSC Done" : Application.DoEvents()
        GetAddedFeatures()
    End Sub

    Private Sub btnExportGenomeRunner_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExportGenomeRunner.Click
        'creates a filedialog for the user to select a place to export the file to
        Dim SaveFD As New SaveFileDialog
        SaveFD.Title = "Select a location to export the genomerunner table to"
        SaveFD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
        SaveFD.FileName = ""
        SaveFD.Filter = "All files|*.*|Text Files|*.txt"
        SaveFD.ShowDialog()
        fileName = SaveFD.FileName

        Dim GRheader As New List(Of String)
        OpenDatabase()
        cmd = New MySqlCommand("SHOW COLUMNS FROM genomerunner;", cn)
        dr = cmd.ExecuteReader
        While dr.Read
            GRheader.Add(dr(0))
        End While
        dr.Close() : cmd.Dispose()

        OpenDatabase()
        cmd = New MySqlCommand("SELECT * FROM genomerunner;", cn)
        dr = cmd.ExecuteReader
        Using writer As StreamWriter = New StreamWriter(fileName)
            writer.WriteLine(String.Join(vbTab, GRheader))
            While dr.Read
                Dim queryAsArray As New ArrayList
                For i As Integer = 0 To dr.FieldCount - 1 Step +1
                    queryAsArray.Add(dr(i).ToString)
                Next
                writer.WriteLine(Join(queryAsArray.ToArray, vbTab))
            End While
        End Using
        dr.Close() : cmd.Dispose()
        lblProgress.Text = "Done" : Application.DoEvents()
    End Sub

    Private Sub btnChangeDataDownloadPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChangeDataDownloadPath.Click
        Dim FolderBrowser As New System.Windows.Forms.FolderBrowserDialog
        FolderBrowser.Description = "Select folder for saving UCSC data files."
        Dim dlgResult As DialogResult = FolderBrowser.ShowDialog()
        If dlgResult = Windows.Forms.DialogResult.OK Then
            DataDownloadPath = FolderBrowser.SelectedPath & "\"
            lblDataDownloadPath.Text = DataDownloadPath
            SaveSetting("Genome Feature Table Creator", "Data", "DataDownloadPath", DataDownloadPath)
        End If
    End Sub

    Private Sub btnGenerateBedFile_Click(sender As System.Object, e As System.EventArgs) Handles btnGenerateBedFile.Click
        Dim processSubfeatures As Integer = 0 '0 - first time encounter, 1 - process Subfeatures, 2 - do not process subfeatures
        For Each Feature In listFeaturesToAdd.SelectedItems
            OpenDatabase()
            cmd = New MySqlCommand("SELECT QueryType,Name FROM genomerunner WHERE FeatureTable='" & Feature & "';", cn)
            dr = cmd.ExecuteReader
            If dr.HasRows Then
                dr.Read()
                Dim distinctName As String = vbNullString
                If dr(1) <> vbNullString And dr(1) <> "NULL" Then
                    distinctName = dr(1)
                    If processSubfeatures = 0 Then
                        Dim response As MsgBoxResult
                        response = MsgBox("Table " & Feature & " has subgroups in column " & distinctName & ". Do you want to process all tables like this into separate subgroups (Yes), or process them as is (No)?", MsgBoxStyle.YesNo, "Process subfeatures separately?")
                        If response = MsgBoxResult.Yes Then processSubfeatures = 1 Else processSubfeatures = 2
                    End If
                    dr.Close() : cmd.Dispose()
                    If processSubfeatures = 1 Then
                        Dim distinctNames As New List(Of String)
                        cmd1 = New MySqlCommand("SELECT DISTINCT " & distinctName & " FROM " & Feature & ";", cn)
                        dr1 = cmd1.ExecuteReader
                        While dr1.Read
                            distinctNames.Add(dr1(0))
                        End While
                        dr1.Close() : cmd1.Dispose()
                        For Each dName In distinctNames
                            cmd1 = New MySqlCommand("SELECT chrom,chromStart,chromEnd,name,score,strand FROM " & Feature & " WHERE " & distinctName & "='" & dName & "';", cn)
                            dr1 = cmd1.ExecuteReader
                            Dim filePath = "F:\" & Feature & "." & dName & ".bed"
                            Using writer As New StreamWriter(filePath.ToString)
                                While dr1.Read
                                    writer.WriteLine(dr1(0) & vbTab & dr1(1) & vbTab & dr1(2) & vbTab & dr1(3) & vbTab & dr1(4) & vbTab & dr1(5))
                                End While
                            End Using
                            dr1.Close() : cmd1.Dispose()
                        Next
                    End If
                Else
                    dr.Close() : cmd.Dispose()
                    lblProgress.Text = "Extracting .bed file for " & Feature
                    Dim filePath = "F:\" & Feature & ".bed"
                    Using writer As StreamWriter = New StreamWriter(filePath.ToString)
                        cmd1 = New MySqlCommand("SELECT chrom,chromStart,chromEnd,name,score,strand FROM " & Feature & ";", cn)
                        dr1 = cmd1.ExecuteReader
                        While dr1.Read
                            writer.WriteLine(dr1(0) & vbTab & dr1(1) & vbTab & dr1(2) & vbTab & dr1(3) & vbTab & dr1(4) & vbTab & dr1(5))
                        End While
                        dr1.Close() : cmd1.Dispose()
                    End Using
                    lblProgress.Text = "Extracting .bed file for " & Feature & " completed"
                End If
                dr.Close() : cmd.Dispose()
            End If
        Next

    End Sub



    Private Sub btnUpdateStatistics_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateStatistics.Click
        'for each entry in genomerunner, count how many records are in that table and save that to count field of genomerunner
        OpenDatabase()

        For Each feature In arrayFeaturesAdded
            lblProgress.Text = feature : Application.DoEvents()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Need to query extra data if this table's genomerunner entry has 'Threshold' as it's QueryType
            Dim thresholdType As String = ""
            Dim thresholdQuery As String = ""
            cmd = New MySqlCommand("SELECT QueryType, ThresholdType FROM genomerunner WHERE FeatureTable = '" & feature & "';", cn)
            dr = cmd.ExecuteReader
            While dr.Read
                'TODO threshold fields only need calculating when QueryType == "Threshold", right??
                If dr(0) = "Threshold" Then
                    thresholdType = dr(1)
                    thresholdQuery = ", thresholdMin = (SELECT MIN(" & thresholdType & ") from " & feature & _
                                     "), thresholdMax = (SELECT MAX(" & thresholdType & ") from " & feature & _
                                     "), thresholdMean = (SELECT AVG(" & thresholdType & ") from " & feature & ")"
                End If
            End While
            dr.Close() : cmd.Dispose()
            'TODO update other calulcated fields as well! Most are too complicated to do in a query.
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Update genomerunner total count and threshold fields
            cmd = New MySqlCommand("UPDATE genomerunner SET count=(SELECT COUNT(*) FROM " & feature & ")" & thresholdQuery & " WHERE FeatureTable = '" & feature & "';", cn)
            cmd.ExecuteNonQuery()
            cmd.Dispose()
        Next
        lblProgress.Text = "Done" : Application.DoEvents()
    End Sub


    '==========================
    'Old code, for possible use
    '==========================

    Private Sub btnReloadFeatures_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReloadFeatures.Click
        DownloadFeatureList()
    End Sub
    'downloads the list of files on the CSC genome annotation and returns them in an array 
    Private Sub DownloadFeatureList()
        Dim ftpFileList As String()
        Dim ftpRequest As FtpWebRequest
        Try
            'requests the features
            ftpRequest = FtpWebRequest.Create(ftpHost)
            ftpRequest.Credentials = New NetworkCredential(ftpUser, "")
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory
            Dim ftpResponse As FtpWebResponse
            ftpResponse = ftpRequest.GetResponse
            'reads the features into a stream
            Dim sr As New StreamReader(ftpRequest.GetResponse().GetResponseStream())
            Dim fList As String() = Split(sr.ReadToEnd(), vbCrLf) 'loads the list of features into a string array
            sr.Close()
            sr = Nothing
            Dim Path As String = System.AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\FeatureList.txt"
            Using sw As New StreamWriter(Path)
                For i As Integer = 0 To fList.Count - 1 Step +1
                    sw.WriteLine(fList(i))
                Next
            End Using
        Catch ex As Exception
            Throw (ex)
        End Try
        GetFeaturesAvailableList()
    End Sub
    'gets the feature list that is saved locally 
    Private Sub GetFeaturesAvailableList()
        'listFeaturesAvailable.Items.Clear() RP this empties out old crap
        Dim Path As String = System.AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\FeatureList.txt"
        Dim sr As New StreamReader(Path)
        Dim fList() As String = Split(sr.ReadToEnd(), vbCrLf)
        For i As Integer = 0 To fList.Count - 1 Step +1 ' adds features to list if not already added
            Dim subStrings As String() = Split(fList(i), ".") 'splits name of the feature file
            If listFeaturesAvailable.Items.Contains(subStrings(0)) = False Then 'adds feature to list if not already added
                listFeaturesAvailable.Items.Add(subStrings(0)) 'adds name of feature without file extension
            End If
        Next
        sr.Close()
        sr = Nothing
    End Sub


    Private Sub btnCreateExonTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExonTable.Click
        If listFeaturesToAdd.SelectedIndex <> -1 Then
            Dim form As New FormExonGenerator
            For Each selfeature In listFeaturesToAdd.SelectedItems
                form.listFeatures.Add(selfeature)
            Next
            form.Text = form.listFeatures(0)
            form.connectionString = ConnectionString
            form.loadFeatureColumns()
            form.ShowDialog()
        Else
            MessageBox.Show("Please select a feature")
        End If
    End Sub

    Private Sub btnStatistics_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatistics.Click
        If listFeaturesToAdd.SelectedIndex <> -1 Then
            Dim form As New FormCalculateStatistics
            For Each selfeature In listFeaturesToAdd.SelectedItems
                form.listFeatures.Add(selfeature)
            Next
            form.Text = form.listFeatures(0)
            form.connectionString = ConnectionString
            form.loadFeatureColumns()
            form.ShowDialog()
        Else
            MessageBox.Show("Please select a feature")
        End If
    End Sub

    Private Sub btnGetCountofBasePairsCovered_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetCountofBasePairsCovered.Click
        Dim bkgChr As String(), bkgStart As Integer(), bkgEnd As Integer()
        Dim TableNames As New List(Of String)
        Dim TotalNumberofBasePairs As Long
        bkgNum = 24 : ReDim bkgChr(bkgNum) : ReDim bkgStart(bkgNum) : ReDim bkgEnd(bkgNum) : backgroundFeatures.Clear()  'prepares the arrays and clears the backgroundfeatuers list
        bkgChr = {"chr1", "chr10", "chr11", "chr12", "chr13", "chr14", "chr15", "chr16", "chr17", "chr18", "chr19", "chr2", "chr20", "chr21", "chr22", "chr3", "chr4", "chr5", "chr6", "chr7", "chr8", "chr9", "chrM", "chrX", "chrY"}
        'bkgStart = {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        bkgStart = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        bkgEnd = {247249719, 135374737, 134452384, 132349534, 114142980, 106368585, 100338915, 88827254, 78774742, 76117153, 63811651, 242951149, 62435964, 46944323, 49691432, 199501827, 191273063, 180857866, 170899992, 158821424, 146274826, 140273252, 16571, 154913754, 57772954}

        'gets the total number of base pairs of the genome
        For Each ChromLength In bkgEnd
            TotalNumberofBasePairs += ChromLength + 1 'adds one to the chrom length as they are zerobased
        Next

        'gets all of the GRfeature table names
        'OpenDatabase()
        'cmd = New MySqlCommand("SELECT featureTable FROM genomerunner WHERE querytype!='NA'", cn)
        'dr = cmd.ExecuteReader()
        'While dr.Read()
        '    TableNames.Add(dr(0))
        'End While
        'dr.Close()
        'adds extra table for exons
        OpenDatabase()
        cmd = New MySqlCommand("SELECT featureTable FROM genomerunner WHERE querytype='Gene'", cn)
        dr = cmd.ExecuteReader()
        While dr.Read()
            TableNames.Add(dr(0) & "Exons")
        End While
        dr.Close()

        'creates a header line
        Using sr As New StreamWriter(AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\FeatureBaseCountReport.txt", True)
            sr.WriteLine("TableName" & vbTab & "Total Base Pairs Covered" & vbTab & "Total Regions Count" & vbTab & "Total Number bp for genome")
        End Using
        ProgressBar1.Maximum = bkgChr.Count
        Dim currTableIndex As Integer = 0
        For Each tName In TableNames
            Dim features As New List(Of Feature)
            Dim totalBasePairHits As Long = 0 'keeps count of the total number of base pairs that are covered by the feature
            Dim TotalFeatureRegionCount As Long = 0 'keeps count of the total number of regions that are 'created' by superimposing the feature regions onto the basepairs of the chromosome
            ProgressBar1.Value = 0
            For currChrom As Integer = 0 To bkgChr.Count - 1
                features.Clear()
                'creates a list to store whether basepair has a hit or not
                Dim BasePairs(bkgEnd(currChrom)) As Boolean
                For currBase As UInteger = 0 To bkgEnd(currChrom)
                    Dim nBase As New ChromBase
                    BasePairs(currBase) = 0 'all base pairs start out as a miss
                Next

                OpenDatabase()
                'gets the start and end position of all of the features on the current chromosome
                Try
                    cmd = New MySqlCommand("SELECT chromStart,ChromEnd FROM " & tName & " WHERE chrom='" & bkgChr(currChrom) & "';", cn)
                    dr = cmd.ExecuteReader()
                Catch
                    cmd = New MySqlCommand("SELECT txStart,txend FROM " & tName & " WHERE chrom='" & bkgChr(currChrom) & "';", cn)
                    dr = cmd.ExecuteReader()
                End Try
                While dr.Read()
                    features.Add(New Feature With {.ChromStart = dr(0), .ChromEnd = dr(1)})
                End While
                dr.Close()

                'goes through each feature and changes the isHit values of the ChromBase list to true for all base
                'pairs that are covered by the features region
                For Each currFeature In features
                    For CurrBase As UInteger = currFeature.ChromStart To currFeature.ChromEnd
                        BasePairs(CurrBase) = True
                    Next
                Next
                Dim stopwatch As New Stopwatch
                stopwatch.Start()
                Dim LastBaseHit As Boolean = False 'keeps track of whether the last base pair was a hit or not
                For CurrBase As Integer = 0 To BasePairs.Count - 1
                    If BasePairs(CurrBase) = True Then
                        totalBasePairHits += 1
                        If LastBaseHit = False Then
                            TotalFeatureRegionCount += 1
                            LastBaseHit = True
                        End If
                    Else
                        LastBaseHit = False
                    End If
                Next
                stopwatch.Stop()
                ProgressBar1.Value += 1 : Application.DoEvents()
            Next

            Using sr As New StreamWriter(AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\FeatureBaseCountReport.txt", True)
                sr.WriteLine(tName & vbTab & totalBasePairHits & vbTab & TotalFeatureRegionCount & vbTab & TotalNumberofBasePairs)
            End Using
            lblProgress.Text = currTableIndex
        Next
    End Sub





    Private Sub Button1_Click_1(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        InputBox("111")
    End Sub




    Private Sub txtDatabase_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtDatabase.TextChanged
        txtUcscdb.Text = txtDatabase.Text
    End Sub

    Private Sub btnFixMMrmsk_Click(sender As System.Object, e As System.EventArgs) Handles btnFixMMrmsk.Click
        Fix_rmsk_table()
        OpenDatabase()
        Dim rmskTables As List(Of String) = New List(Of String), rmskTime As String
        cmd = New MySqlCommand("SELECT FeatureTable,time FROM genomerunner WHERE FeatureTable like '%rmsk';", cn)
        dr = cmd.ExecuteReader
        If dr.HasRows Then
            While dr.Read
                rmskTables.Add(dr(0))
                rmskTime = dr(1)
            End While
        Else
            dr.Close() : cmd.Dispose() : Exit Sub
        End If
        dr.Close() : cmd.Dispose()

        cmd = New MySqlCommand("CREATE TABLE  IF NOT EXISTS rmsk (" & _
                            "bin smallint(5) unsigned NOT NULL DEFAULT 0," & _
                            "swScore int(10) unsigned NOT NULL DEFAULT 0," & _
                            "milliDiv int(10) unsigned NOT NULL DEFAULT 0," & _
                            "milliDel int(10) unsigned NOT NULL DEFAULT 0," & _
                            "milliIns int(10) unsigned NOT NULL DEFAULT 0," & _
                            "genoName varchar(255) NOT NULL DEFAULT ''," & _
                            "genoStart int(10) unsigned NOT NULL DEFAULT 0," & _
                            "genoEnd int(10) unsigned NOT NULL DEFAULT 0," & _
                            "genoLeft int(11) NOT NULL DEFAULT 0," & _
                            "strand char(1) NOT NULL DEFAULT ''," & _
                            "repName varchar(255) NOT NULL DEFAULT ''," & _
                            "repClass varchar(255) NOT NULL DEFAULT ''," & _
                            "repFamily varchar(255) NOT NULL DEFAULT ''," & _
                            "repStart int(11) NOT NULL DEFAULT 0," & _
                            "repEnd int(11) NOT NULL DEFAULT 0," & _
                            "repLeft int(11) NOT NULL DEFAULT 0," & _
                            "id char(1) NOT NULL DEFAULT ''," & _
                            "KEY bin (bin)" & _
                            ")", cn)
        cmd.ExecuteNonQuery() : cmd.Dispose()
        Fix_rmsk_table()

        For Each rTable In rmskTables
            'decompresses this feature's .gz file to a .txt file
            If File.Exists(DataDownloadPath & rTable.ToString & ".txt") = False Then
                Using outfile As FileStream = File.Create(DataDownloadPath & rTable.ToString & ".txt")
                    Using infile As FileStream = File.OpenRead(DataDownloadPath & rTable.ToString & ".txt.gz")
                        Using Decompress As System.IO.Compression.GZipStream = New System.IO.Compression.GZipStream(infile, Compression.CompressionMode.Decompress)
                            Decompress.CopyTo(outfile)
                        End Using
                    End Using
                End Using
                cmd = New MySqlCommand("LOAD DATA LOCAL INFILE '" & DataDownloadPath.Replace("\", "/") & rTable.ToString & ".txt" & "' INTO TABLE rmsk;", cn)
                cmd.ExecuteNonQuery() : cmd.Dispose()
                File.Delete(DataDownloadPath & rTable.ToString & ".txt")
            End If
        Next

        cmd = New MySqlCommand("INSERT INTO genomerunner (id, FeatureTable, FeatureName, QueryType, ThresholdType, Tier, Category, orderofcategory, Name, complete, time) VALUES (" & _
                                                    "100002,'rmsk','Repeating Elements by RepeatMasker Combined','OutputScore','repName',2,'09|Variation and Repeats',0,'repFamily',31,'" & rmskTime & "');", cn)
        cmd.ExecuteNonQuery() : cmd.Dispose()
    End Sub

End Class

Class ChromBase
    Public IsHit As Boolean
End Class



