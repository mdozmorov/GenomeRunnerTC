Imports MySql.Data.MySqlClient
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
    Dim fileName As String, numOfFeaturesToAdd As Integer = 0, progress As Integer = 1 'stores the value of the progress bar
    Dim ConnectionString As String
    Dim Features As New List(Of Feature)
    Dim RandomFeatures As New List(Of Feature)
    Dim backgroundFeatures As New List(Of Feature)
    Dim featureTables As New List(Of String)
    Dim bkgNum As Integer
    Private DataDownloadPath As String = ""

    'connects to the db
    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        btnExonTable.Enabled = True
        btnGenerateBackground.Enabled = True
        listFeaturesToAdd.Items.Clear() 'clears the list
        Try
            OpenDatabase()
            'RP: Commented out this next line b/c it should use DownloadFeatureList() first;
            '    otherwise it's just using a previously saved file that doesn't necessarily
            '    correspond to the currently selected UCSC database.
            '    For now this is handled when the user clicks "Update Features List".
            'GetFeaturesAvailableList() 'gets list of features that can be added

            GetAddedFeatures() 'loads the table names of features added to list
            GetEmptyFeatures() 'checks for features in the db that are empty
            syncGenomeTableToDatabase() 'checks for features that have been added to the genomerunner table but not actually added to the database
        Catch
            MessageBox.Show("Please check connection settings and try again")
        End Try
    End Sub

    'opens a connection to the database; uses values supplied by the user
    Private Sub OpenDatabase()
        Dim uName As String = txtUser.Text
        Dim uServer As String = txtHost.Text
        Dim uPassword As String = txtPassword.Text
        Dim uDatabase As String = txtDatabase.Text
        Dim uUcscDatabaseName As String = txtUcscdb.Text

        ' If uName <> "" And uPassword <> "" And uServer <> "" And uDatabase <> "" Then
        ConnectionString = "Server=" & uServer & ";Database=" & uDatabase & ";User ID=" & uName & ";Password=" & uPassword & ";Connection Timeout=6000;default command timeout=600" 'uses the values provided by user to create constring. TODO add error checking
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
    Private Sub FormMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReloadFeatures.Click
        DownloadFeatureList()
    End Sub

    'executes all of the changes that have been set by the user
    Private Sub btnAddFeatures_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'OpenDatabase()
        'numOfFeaturesToAdd = arrayFeaturesToAdd.Count
        'ProgressBar1.Maximum = numOfFeaturesToAdd
        'If arrayFeaturesToAdd.Count <> 0 Then
        '    LabelStatusBar.Text = "Downloading files from " & ftpHost
        '    MessageBox.Show("Downloading files from " & ftpHost)
        '    DownloadFeatures() 'downloads the selected features
        '    LabelStatusBar.Text = "Decompressing files"
        '    MessageBox.Show("Decompressing files")
        '    DecompressFeatures() 'decompresses the features
        '    LabelStatusBar.Text = "Loading Feature Tables To Database"
        '    MessageBox.Show("Going to add features to database")
        '    AddFeaturesToDatabase(arrayFeaturesToAdd) 'loads the data into the database
        'End If
        'If arrayFeaturesToRemove.Count <> -1 Then
        '    LabelStatusBar.Text = "Removing selected features"
        '    MessageBox.Show("Going to remove " & arrayFeaturesToRemove.Count & " features")
        '    RemoveFeatures(arrayFeaturesToRemove)
        '    MessageBox.Show("Changes successfully executed")
        'End If
        'cn.Close()
        'arrayFeaturesToAdd.Clear()
        'arrayFeaturesAdded.Clear()
        'listFeaturesToAdd.Items.Clear()
        'GetAddedFeatures()
        'AddHandler listFeaturesAvailable.DrawItem, AddressOf listFeaturesAvailable_DrawItem
        'AddHandler listFeaturesToAdd.DrawItem, AddressOf listFeaturesToAdd_DrawItem
    End Sub
    'Adds the selected feature feature to add list
    Private Sub btnAddToList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddToList.Click
        'adds the feature to the to arrayFeaturetoAdd if it does not already exist there or in the arrayFeaturesAdded
        For i As Integer = 0 To listFeaturesAvailable.SelectedItems.Count - 1 Step +1
            If arrayFeaturesToAdd.IndexOf(listFeaturesAvailable.SelectedItems.Item(i)) = -1 And arrayFeaturesAdded.IndexOf(listFeaturesAvailable.SelectedItems.Item(i).ToString.ToLower()) = -1 Then
                arrayFeaturesToAdd.Add(listFeaturesAvailable.SelectedItems.Item(i)) 'stores the feature to feature to add array
            End If
        Next
        SyncFeatureToAddListToArrays()
        listFeaturesAvailable.ClearSelected()
        listFeaturesToAdd.ClearSelected()
    End Sub

    'sets the feature to be removed 
    Private Sub btnRemoveFromList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveFromList.Click
        Dim response As Boolean = MsgBox("Are you sure you want to remove the selected features from the database?", vbYesNo) 'asks the user if they want to overwrite the old query with the new
        'If response = vbYes Then
        Try
            For Each feature In listFeaturesToAdd.SelectedItems
                RemoveFeatures(feature)
            Next
        Catch ex As Exception
            Throw ex
        End Try
        ' End If
        GetAddedFeatures()
        GetEmptyFeatures()
        syncGenomeTableToDatabase()
    End Sub

    'sets the settings to not be added
    Private Sub btnRevert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        arrayFeaturesAdded.Clear()
        arrayFeaturesToRemove.Clear()
        listFeaturesAvailable.ClearSelected()
        listFeaturesToAdd.ClearSelected()
    End Sub
    'get features already added to the genomerunnertable
    Private Sub GetAddedFeatures()
        OpenDatabase()
        arrayFeaturesAdded.Clear()
        arrayFeaturesToUpdate.Clear()
        listFeaturesToAdd.Items.Clear()
        cmd = New MySqlCommand("SELECT featuretable FROM genomerunner", cn)
        Dim dr As MySqlDataReader
        dr = cmd.ExecuteReader()
        While dr.Read()
            arrayFeaturesAdded.Add(dr(0))
            listFeaturesToAdd.Items.Add(arrayFeaturesAdded(arrayFeaturesAdded.Count - 1))
        End While
        cn.Close() : dr.Close()
    End Sub
    'get empty feature tables
    Private Sub GetEmptyFeatures()
        OpenDatabase()
        arrayFeaturesEmpty.Clear()
        For Each item In listFeaturesToAdd.Items
            cmd = New MySqlCommand("SELECT * FROM " & item & " LIMIT 1", cn)
            dr = cmd.ExecuteReader()
            If dr.HasRows = False Then
                arrayFeaturesEmpty.Add(item)
            End If
            dr.Close()
        Next
        cn.Close()
    End Sub
    'syncs the listFeaturesToAdd to both the array of features to add and features added
    Private Sub SyncFeatureToAddListToArrays()
        listFeaturesToAdd.Items.Clear()
        For i As Integer = 0 To arrayFeaturesToAdd.Count - 1 Step +1
            listFeaturesToAdd.Items.Add(arrayFeaturesToAdd(i))
        Next
        For i As Integer = 0 To arrayFeaturesAdded.Count - 1 Step +1
            listFeaturesToAdd.Items.Add(arrayFeaturesAdded(i))
        Next
    End Sub

    'downloads and decompresses the feature 
    Private Sub PrepareFeatures(ByVal feature As String)
        Try
            If File.Exists(DataDownloadPath & feature & ".sql") = False Then
                lblProgress.Text = "Downloading " & feature & ".sql" : Application.DoEvents()
                My.Computer.Network.DownloadFile(ftpHost & feature & ".sql", DataDownloadPath & feature & ".sql") 'downloads the .sql feature file
            End If
            If File.Exists(DataDownloadPath & feature & ".txt.gz") = False Then
                lblProgress.Text = "Downloading " & feature & ".txt.gz" : Application.DoEvents()
                My.Computer.Network.DownloadFile(ftpHost & feature & ".txt.gz", DataDownloadPath & feature & ".txt.gz") 'downloads the .txt.gz feature file

            End If
            'decompresses the .gz file to a .txt file
            If File.Exists(DataDownloadPath & feature & ".txt") = False Then
                lblProgress.Text = "Decompressing " & feature : Application.DoEvents()
                Using outfile As FileStream = File.Create(DataDownloadPath & feature & ".txt")
                    Using infile As FileStream = File.OpenRead(DataDownloadPath & feature & ".txt.gz")
                        Using Decompress As System.IO.Compression.GZipStream = New System.IO.Compression.GZipStream(infile, Compression.CompressionMode.Decompress)
                            Decompress.CopyTo(outfile)
                        End Using
                    End Using
                End Using
            End If
        Catch
            MessageBox.Show("Unable to download files for " & feature)
        End Try
    End Sub

    'adds the features to the database
    Private Sub AddFeaturesToDatabase(ByVal feature As String)
        Dim filePathFeaturesToAdd = DataDownloadPath
        Dim filestream As String = File.ReadAllText(filePathFeaturesToAdd & feature & ".sql")
        Dim query As String = ""
        If File.Exists(filePathFeaturesToAdd & ".sql") = True And File.Exists(filePathFeaturesToAdd & feature & ".txt.gz") = True Or File.Exists(filePathFeaturesToAdd & feature & ".txt") Then 'continues if both required files are found
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
                    querySuccessful = True 'sets query status as successful. causes the loop to end
                    If queryDidFail = True Then
                        Dim response As Boolean = MsgBox("Query successful, do  you want to overwrite the old query file?", vbYesNo) 'asks the user if they want to overwrite the old query with the new
                        If response = True Then
                            File.WriteAllText(filePathFeaturesToAdd & feature & ".sql", query) 'replaces the query file with a new query
                        End If
                    End If

                    Try
                        'checks if the feature name already exists in the genomerunner table
                        cmd = New MySqlCommand("SELECT featuretable FROM genomerunner WHERE featuretable = '" & feature & "'", cn)
                        dr = cmd.ExecuteReader()
                        If dr.HasRows <> True Then
                            dr.Close() : cmd.Dispose()
                            'adds the table name to the GenomeRunner table
                            cmd = New MySqlCommand("INSERT INTO genomerunner ( FeatureTable,FeatureName) VALUES ('" & feature & "','" & feature & "')", cn)
                            cmd.ExecuteNonQuery()
                        End If

                    Catch e As Exception : Throw e

                    End Try
                Catch ex As Exception
                    Dim form As New FormMysqlQueryEditor() 'creates a form for the user to change the query
                    form.originQuery = filestream.Replace("longblob", "longtext")
                    form.LabelStatus.Text = "There was an error executing the query.  Please review the query for proper formatting and resubmit."
                    form.LabelErrorMessage.Text = "Error Message: " & ex.Message
                    form.ShowDialog()
                    If form.skipQuery = True Then
                        arrayFeaturesNotAddedSuccessfully.Add(feature) 'adds the feature to list of features that failed to be added
                        GoTo End_Loop 'skips the query
                    End If
                    query = form.editedQuery
                    queryDidFail = True
                End Try
                cn.Close()
            End While
        End If
End_Loop:
    End Sub

    Private Sub PopulateDatabse(ByVal feature As String)
        OpenDatabase()

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
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
        dr.Close()
        cmd.Dispose()

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Populate
        Dim filePathFeaturesToAdd = DataDownloadPath
        Dim filePathFeatureToAddUNIX As String = filePathFeaturesToAdd.Replace("\", "/") 'modifies the filepath to conform to UNIX 
        Dim query As String = "LOAD DATA LOCAL INFILE " & "'" & filePathFeatureToAddUNIX & feature & ".txt' INTO TABLE " & feature & ";" & _
                              "UPDATE genomerunner SET count = (SELECT COUNT(*) FROM " & feature & ")" & thresholdQuery & " WHERE FeatureTable = '" & feature & "';"
        Try
            cmd = New MySqlCommand(query, cn) 'reads the .txt file and inserts the data into the created table
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Dim form As New FormQuery
            form.txtQuery.Text = query
            form.ShowDialog()

        End Try
        cmd.Dispose() : cn.Close() : dr.Close()
        'Now delete unzipped .txt file to save space.
        File.Delete(filePathFeaturesToAdd & feature & ".txt")
    End Sub

    Private Sub CreateExonTable(ByVal feature As String)
        Dim exons As List(Of Exon)
        OpenDatabase()
        cmd = New MySqlCommand("SELECT QueryType FROM genomerunner WHERE FeatureTable = '" & feature & "';", cn)
        dr = cmd.ExecuteReader
        dr.Read()
        Dim queryType = dr(0)
        cmd.Dispose() : dr.Close()
        'Creating exon table is only necessary if queryType = "Gene"
        If queryType = "Gene" Then
            exons = GetExons(feature)
            CreateTableFromExons(feature, exons)
        End If
        cn.Close()
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
        lblProgress.Text = "Creating new table called " & feature & "Exons" : Application.DoEvents()
        OpenDatabase()
        cmd = New MySqlCommand("DROP TABLE IF EXISTS " & feature & "Exons; CREATE  TABLE `" & feature & "Exons` (`name` VARCHAR(50) NULL , `chrom` VARCHAR(45) NULL, `strand` VARCHAR(45) NULL ,  `txStart` INT NULL ,  `txEnd` INT NULL ,  `exonStart` INT NULL ,  `exonEnd` INT NULL); ", cn)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
        cn.Close() : dr.Close()

        'reads the data in the text file into the new exon table
        Dim query As String = ""
        lblProgress.Text = "Creating new table called " & feature & "Exon" : Application.DoEvents()
        Try
            'clears all data that might have existed in the Exon table
            OpenDatabase()
            query = "TRUNCATE TABLE " & feature & "Exons;"
            cmd = New MySqlCommand(query, cn)
            cmd.ExecuteNonQuery()
            cmd.Dispose() : cn.Close() : dr.Close()
            OpenDatabase()
            'loads the new data into the exon table
            Dim filePathFeatureToAddUNIX As String = filepath.Replace("\", "/") 'modifies the filepath to conform to UNIX
            query = "LOAD DATA LOCAL INFILE " & "'" & filePathFeatureToAddUNIX & "' INTO TABLE " & feature & "Exons;"
            cmd = New MySqlCommand(query, cn)
            cmd.ExecuteNonQuery()
            cmd.Dispose() : cn.Close() : dr.Close()
        Catch
            Dim form As New FormQuery
            form.txtQuery.Text = query
            form.ShowDialog()
        End Try
    End Sub

    'removes features from the database
    Private Sub RemoveFeatures(ByVal feature As String)
        OpenDatabase()
        cmd = New MySqlCommand("DROP TABLE IF EXISTS " & feature, cn)
        cmd.ExecuteNonQuery()
        cmd = New MySqlCommand("DELETE FROM genomerunner WHERE featuretable = '" & feature & "';", cn)
        cmd.ExecuteNonQuery()
    End Sub

    'changes the background color of items in the listAvailable that are already added 
    Private Sub listFeaturesToAdd_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles listFeaturesToAdd.DrawItem
        e.DrawBackground()
        'changes the color of the list items
        'TODO add light green for existing items that need to be updated. Use arrayFeaturesToUpdate
        e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault)
        If arrayFeaturesToAdd.IndexOf(listFeaturesToAdd.Items.Item(e.Index)) <> -1 Then 'searches if the feature in the list of features in the db is in the array of feastures to add
            e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, Brushes.BlueViolet, e.Bounds, StringFormat.GenericDefault) 'paints the text green
        ElseIf arrayFeaturesToRemove.IndexOf(listFeaturesToAdd.Items(e.Index).ToString()) <> -1 Then 'searches if the feature in the list of features from thd db is in the list of feature to remove
            e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, Brushes.Red, e.Bounds, StringFormat.GenericDefault) 'paints the text red
        ElseIf arrayFeaturesToUpdate.IndexOf(listFeaturesToAdd.Items(e.Index).ToString()) <> -1 Then 'searches if the feature in the list of features from thd db is in the list of feature to update
            e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, Brushes.LightGreen, e.Bounds, StringFormat.GenericDefault) 'paints the text light green
        ElseIf arrayFeaturesNearlyUpToDate.IndexOf(listFeaturesToAdd.Items(e.Index).ToString()) <> -1 Then
            e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, Brushes.Blue, e.Bounds, StringFormat.GenericDefault) 'paints the text light green
        End If
        If arrayFeaturesEmpty.IndexOf(listFeaturesToAdd.Items.Item(e.Index)) <> -1 Then
            e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, Brushes.Orange, e.Bounds, StringFormat.GenericDefault)
        End If
        e.DrawFocusRectangle()
    End Sub

    Private Sub listFeaturesAvailable_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles listFeaturesAvailable.DrawItem
        e.DrawBackground()
        'changes the color of the list items 
        e.Graphics.DrawString(listFeaturesAvailable.Items(e.Index).ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault) 'makes the default text color black
        If arrayFeaturesAdded.IndexOf(listFeaturesAvailable.Items.Item(e.Index).ToString.ToLower()) <> -1 Then 'checks if the feature is in the array of features added
            e.Graphics.DrawString(listFeaturesAvailable.Items(e.Index).ToString(), e.Font, Brushes.Green, e.Bounds, StringFormat.GenericDefault) 'paints the text green
        End If
        If arrayFeaturesToAdd.IndexOf(listFeaturesAvailable.Items.Item(e.Index).ToString().ToLower()) <> -1 Then 'checks if the feature is in the array of features to add
            e.Graphics.DrawString(listFeaturesAvailable.Items(e.Index).ToString(), e.Font, Brushes.GreenYellow, e.Bounds, StringFormat.GenericDefault) 'paints the text yelllow
        End If
        e.DrawFocusRectangle()
    End Sub

    'loads a list of features to add from a .txt file
    Private Sub btnLoadList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadList.Click
        arrayFeaturesToAdd.Clear() 'clears the lsit of feature to add in order to avoid duplicates
        Dim OpenFD As New OpenFileDialog
        OpenFD.Title = "Select a .txt file with list of features to be added"
        OpenFD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
        OpenFD.FileName = ""
        OpenFD.Filter = "All files|*.*|Text Files|*.txt"
        OpenFD.ShowDialog()
        fileName = OpenFD.FileName
        If fileName <> "" Then
            Using reader As StreamReader = New StreamReader(fileName) 'reads the list of features
                While Not reader.EndOfStream
                    Dim feature As String = reader.ReadLine
                    If listFeaturesToAdd.Items.IndexOf(feature) = -1 Then 'checks if feature not already added
                        arrayFeaturesToAdd.Add(feature) 'adds the features to the array
                    End If
                End While
            End Using
            SyncFeatureToAddListToArrays() 'syncs the list of features to be added and thos already added
        End If
    End Sub

    'exports the items in the both the array of features to add as well as the array of feature added with correct casing from the list of features available
    Private Sub btnExportList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExportList.Click
        Dim arrayFeaturesToExport As New ArrayList() 'stores the list of features to export to list
        For Each feature In arrayFeaturesToAdd 'cycles through array of feature to add
            For x As Integer = 0 To listFeaturesAvailable.Items.Count - 1 Step +1 'goes through items in list of available feature and checks if the array of feature to add exists
                If listFeaturesAvailable.Items(x).ToString.ToLower() = feature Then '
                    arrayFeaturesToExport.Add(listFeaturesAvailable.Items(x))
                End If
            Next
        Next
        For Each feature In arrayFeaturesAdded 'cycles through array of feastures added
            For x As Integer = 0 To listFeaturesAvailable.Items.Count - 1 Step +1 'goes through items in list of available feature and checks if the array of feature to add exists
                If listFeaturesAvailable.Items(x).ToString.ToLower() = feature Then '
                    arrayFeaturesToExport.Add(listFeaturesAvailable.Items(x)) 'adds the item to the array of features to export
                End If
            Next
        Next
        'creates a filedialog for the user to select a place to export the list to
        Dim SaveFD As New SaveFileDialog
        SaveFD.Title = "Select a location to export the list to"
        SaveFD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
        SaveFD.FileName = ""
        SaveFD.Filter = "All files|*.*|Text Files|*.txt"
        SaveFD.ShowDialog()
        fileName = SaveFD.FileName
        Using writer As StreamWriter = New StreamWriter(fileName)
            For Each feature In arrayFeaturesToExport
                writer.WriteLine(feature)
            Next
        End Using
    End Sub

    'checks if the feature actually exists in the database as a table, if not then it is added to the list of features to add
    Private Sub syncGenomeTableToDatabase()
        OpenDatabase()

        'gets all of the feature tables
        cmd = New MySqlCommand("SHOW tables;", cn)
        Dim dr As MySqlDataReader
        dr = cmd.ExecuteReader()
        While dr.Read()
            featureTables.Add(dr(0))
        End While
        'goes through each of the features that are listed in the genomerunner table and adds them to the list of features to add if their table does not exist
        For Each feature In arrayFeaturesAdded
            If featureTables.IndexOf(feature.ToString().ToLower()) = -1 Then
                arrayFeaturesToAdd.Add(feature)
            End If
        Next
        cn.Close() : dr.Close()

    End Sub

    Private Sub btnPrepareFiles_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrepareFiles.Click
        ProgressBar1.Maximum = arrayFeaturesToAdd.Count + arrayFeaturesToUpdate.Count + arrayFeaturesEmpty.Count
        ProgressBar1.Value = 0
        For Each featureToAdd In arrayFeaturesToAdd
            PrepareFeatures(featureToAdd)
            ProgressBar1.Value += 1
        Next
        For Each featureToUpdate In arrayFeaturesToUpdate
            PrepareFeatures(featureToUpdate)
            ProgressBar1.Value += 1
        Next
        For Each emptyFeature In arrayFeaturesEmpty
            PrepareFeatures(emptyFeature)
            ProgressBar1.Value += 1
        Next
        lblProgress.Text = "Done"
        ProgressBar1.Value = 0
        syncGenomeTableToDatabase()
    End Sub

    Private Sub btnCreateTables_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateTables.Click
        ProgressBar1.Maximum = arrayFeaturesToAdd.Count + arrayFeaturesToUpdate.Count + arrayFeaturesEmpty.Count
        ProgressBar1.Value = 0
        Dim arrayAddedSuccessfully As New ArrayList
        For Each featureToAdd In arrayFeaturesToAdd
            AddFeaturesToDatabase(featureToAdd)  'runs the .sql query 
            ProgressBar1.Value += 1
        Next
        For Each featureToUpdate In arrayFeaturesToUpdate
            AddFeaturesToDatabase(featureToUpdate)  'runs the .sql query 
            ProgressBar1.Value += 1
        Next
        For Each emptyFeature In arrayFeaturesEmpty
            AddFeaturesToDatabase(emptyFeature)
            ProgressBar1.Value += 1
        Next
        arrayFeaturesToAdd.Clear()
        arrayFeaturesToUpdate.Clear()
        For Each feature In arrayFeaturesNotAddedSuccessfully 're-adds all of the features not successfully added to the array of features to add
            arrayFeaturesToAdd.Add(feature)
        Next
        arrayFeaturesNotAddedSuccessfully.Clear()
        lblProgress.Text = "Done"
        ProgressBar1.Value = 0
        GetAddedFeatures() 'loads the table names of features added to list
        SyncFeatureToAddListToArrays()
        GetEmptyFeatures()
    End Sub

    Private Sub btnLoadData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadData.Click
        ProgressBar1.Maximum = arrayFeaturesEmpty.Count
        ProgressBar1.Value = 0
        GetEmptyFeatures()
        For Each emptyFeature In arrayFeaturesEmpty
            lblProgress.Text = "Loading data for " & emptyFeature : Application.DoEvents()
            PopulateDatabse(emptyFeature)
            CreateExonTable(emptyFeature)
        Next
        GetEmptyFeatures()
        lblProgress.Text = "Done"
        ProgressBar1.Value = 0
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

    Private Sub btnGenerateBackground_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerateBackground.Click
        If listFeaturesToAdd.SelectedIndex <> -1 Then
            Features.Clear()
            RandomFeatures.Clear()
            backgroundFeatures.Clear()
            OpenDatabase()
            Dim currTable As String = listFeaturesToAdd.SelectedItem  'gets the currently selected feature
            cmd = New MySqlCommand("TRUNCATE TABLE Random;", cn)  'completely erases the random table to prepare for the generation of new values
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            'tries various queries             
            Try
                cmd = New MySqlCommand("SELECT chrom,chromstart,chromend FROM " & currTable, cn)
                dr = cmd.ExecuteReader()
            Catch
                Try
                    cmd = New MySqlCommand("SELECT chrom,ExonStart,ExonEnd FROM " & currTable, cn) 'this must be done before the txstart,end query as the Exon table is the only one with an ExonEnd which differentiates it from the RefGene table
                    dr = cmd.ExecuteReader()

                Catch
                    Try
                        cmd = New MySqlCommand("SELECT chrom,txStart,txEnd FROM " & currTable, cn)
                        dr = cmd.ExecuteReader()
                    Catch ex As Exception
                        MessageBox.Show("Failed to generate random feature")
                    End Try
                End Try
            End Try
            'adds the results to the a list of features
            While dr.Read()
                Dim feature As New Feature
                feature.Chrom = dr(0)
                feature.ChromStart = dr(1)
                feature.ChromEnd = dr(2)
                Features.Add(feature)
            End While
            cmd.Dispose() : cn.Close() : dr.Close()
            SetGenomeBackground()
            SetRandFeatureInterval1(Features.Count - 1)
            AddRandomToDatabase()
            lblProgress.Text = "Done"
            MessageBox.Show("Random feature generated")
        Else
            MessageBox.Show("Please Select a feature from which to generate the random background from the list of features")
        End If
    End Sub

    Public Sub SetGenomeBackground()
        Dim bkgChr As String(), bkgStart As Integer(), bkgEnd As Integer(),
        bkgNum = 24 : ReDim bkgChr(bkgNum) : ReDim bkgStart(bkgNum) : ReDim bkgEnd(bkgNum) : backgroundFeatures.Clear()  'prepares the arrays and clears the backgroundfeatuers list
        bkgChr = {"chr1", "chr10", "chr11", "chr12", "chr13", "chr14", "chr15", "chr16", "chr17", "chr18", "chr19", "chr2", "chr20", "chr21", "chr22", "chr3", "chr4", "chr5", "chr6", "chr7", "chr8", "chr9", "chrM", "chrX", "chrY"}
        'bkgStart = {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        bkgStart = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        bkgEnd = {247249719, 135374737, 134452384, 132349534, 114142980, 106368585, 100338915, 88827254, 78774742, 76117153, 63811651, 242951149, 62435964, 46944323, 49691432, 199501827, 191273063, 180857866, 170899992, 158821424, 146274826, 140273252, 16571, 154913754, 57772954}
        For i As Integer = 0 To bkgNum Step +1
            Dim feature As New Feature
            feature.Chrom = bkgChr(i)
            feature.ChromStart = bkgStart(i)
            feature.ChromEnd = bkgEnd(i)
            backgroundFeatures.Add(feature)
        Next

    End Sub

    Private Sub SetRandFeatureInterval1(ByVal NumOfFeatures)
        'LC 6/20/11 changed to generate random regions
        'Generating random coordinates using Alglib package
        Dim CurrBkgChr As UInteger, CurrBkgIntervalStart As Integer, CurrBkgIntervalEnd As Integer, CurrBkgIntervalLength As Integer, state As hqrndstate
        Dim CurrBkgBufferEnd As Integer 'LC 6/20/11 is the max endpoint that can be selected that allows for the entire region to fit within the chromosome
        Dim currFeature As Integer = 0 'used to cycle through first half of array to get values of the FOI 
        ' ReDim RandChr(NumOfFeatures) : ReDim RandStart(NumOfFeatures) : ReDim RandEnd(NumOfFeatures)       'LC 6/20/11 Prepare arrays for random features: not needed as values are store in the feature array
        'Erase randEventsCount : ReDim randEventsCount(NumOfFeatures + 1)        'Prepare array for counting features
        hqrndrandomize(state)                                               'Initialize random number generator
        For i As Integer = 0 To NumOfFeatures Step +1                                  'LC 6/20/11 Same number of random features as for experimental, start at second half of array 
            Dim randomFeature As New Feature 'stores the random feature generated and is added to the list of Random Features
            'For currFeature = 0 To NumOfFeatures Step +1 'LC 6/22/11 not needed  'LC 6/21/11 cycles through the first half of the array to get the lenghts of the regions
            CurrBkgChr = hqrnduniformi(state, backgroundFeatures.Count)                   'Select random chromosome from 0 through bkgNum-1
            CurrBkgIntervalLength = Features(i).ChromEnd - Features(i).ChromStart  'gets the length of the FOI in order to create a random feature of the same length(this was calculated earlier and stored in an array before FIO start and end arrays were errased)

            'Random intraval coordinate: random number from 0 through [End-Length]
            CurrBkgBufferEnd = BackgroundFeatures(CurrBkgChr).ChromEnd - CurrBkgIntervalLength  'is a buffer that prevents the region from being larger than the chromosome
            CurrBkgIntervalStart = hqrnduniformi(state, backgroundFeatures(CurrBkgChr).ChromEnd - backgroundFeatures(CurrBkgChr).ChromStart) + backgroundFeatures(CurrBkgChr).ChromStart
            If CurrBkgBufferEnd >= 0 Then 'checks to see if the startpoint is not negative 
                While CurrBkgIntervalStart > CurrBkgBufferEnd 'LC 6/20/11 added in case the random startpoint fell within the buffer region
                    CurrBkgIntervalStart = hqrnduniformi(state, BackgroundFeatures(CurrBkgChr).ChromEnd - BackgroundFeatures(CurrBkgChr).ChromStart + 1) + BackgroundFeatures(CurrBkgChr).ChromStart 'LC 6/20/11 changed to prevent random startpoint that falls within the end region "---" |......---|
                End While
                CurrBkgIntervalEnd = CurrBkgIntervalStart + CurrBkgIntervalLength  'LC 6/20/11 the endpoint is start+length of feature
                randomFeature.Chrom = BackgroundFeatures(CurrBkgChr).Chrom                           'Store CurrBkgChr chromosome
                randomFeature.ChromStart = CurrBkgIntervalStart                            'and corresponding random coordinate within it
                randomFeature.ChromEnd = CurrBkgIntervalEnd                           'LC 6/20/11 added
            Else 'LC 6/20/11 if the startpoint is negative, then the region is larger than the chromosome and so the region is set to be entire region of chromosome   
                randomFeature.Chrom = BackgroundFeatures(CurrBkgChr).Chrom
                randomFeature.ChromStart = BackgroundFeatures(CurrBkgChr).ChromStart
                randomFeature.ChromEnd = BackgroundFeatures(CurrBkgChr).ChromEnd
            End If
            RandomFeatures.Add(randomFeature)  'adds the new random feature to the list
            currFeature += 1
        Next
    End Sub

    Private Sub AddRandomToDatabase()
        lblProgress.Text = "Loading randomly generated background into database" : Application.DoEvents()
        Dim filpath As String = Application.StartupPath & "\randomoutput.txt"
        Using writer As StreamWriter = New StreamWriter(filpath)
            For Each Feature In RandomFeatures
                Dim outputline As String = Feature.Chrom & vbTab & Feature.ChromStart & vbTab & Feature.ChromEnd & vbTab & "NA" & vbTab & "NA"
                writer.WriteLine(outputline)
            Next
        End Using
        OpenDatabase()
        cmd = New MySqlCommand("DROP TABLE IF EXISTS `randomfeature`;CREATE TABLE `randomfeature` (`Chrom` varchar(45) DEFAULT NULL,`ChromStart` int(11) DEFAULT NULL,`ChromEnd` int(11) DEFAULT NULL, `strand` varchar(45) DEFAULT 'NA', `name` varchar(45) DEFAULT 'NA',`id` int(11) NOT NULL AUTO_INCREMENT, PRIMARY KEY (`id`)) ENGINE=MyISAM DEFAULT CHARSET=utf8;", cn)
        cmd.ExecuteNonQuery()
        Dim filePathFeatureToAddUNIX As String = filpath.Replace("\", "/") 'modifies the filepath to conform to UNIX
        Dim query As String = "LOAD DATA LOCAL INFILE " & "'" & filePathFeatureToAddUNIX & "' INTO TABLE RandomFeature;"
        Try
            cmd = New MySqlCommand(query, cn) 'reads the .txt file and inserts the data into the created table
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Dim form As New FormQuery
            form.txtQuery.Text = query
            form.ShowDialog()

        End Try
        cn.Close()
        'OpenDatabase()
        'Dim query As String = "INSERT INTO random (chrom,chromstart,chromend) values "
        'ProgressBar1.Value = 0
        'ProgressBar1.Maximum = RandomFeatures.Count
        'For x As Integer = 0 To RandomFeatures.Count
        '    query += "('" & RandomFeatures(x).Chrom & "', " & "'" & RandomFeatures(x).ChromStart & "', " & RandomFeatures(x).ChromEnd & "') "
        '    ProgressBar1.Value += 1 : Application.DoEvents()
        'Next
        'query += ";"
        'cmd = New MySqlCommand(query, cn)
        'cmd.ExecuteNonQuery()
        'cmd.Dispose() : cn.Close()
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

    Private Sub btnGenerateBedFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerateBedFile.Click

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

    Private Sub btnCompareToUCSC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCompareToUCSC.Click
        
        'NOTE: This command will get row count for each table in the selected database.
        '"SELECT table_name, TABLE_ROWS FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'mm9test';"
        'UCSC db info:
        'mysql --user=genome --host=genome-mysql.cse.ucsc.edu -A

        Dim tableNamesForUCSCQuery As New List(Of String)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Row count from local genomerunner
        Dim localTablesToCounts As New Hashtable 'key is FeatureTable, value is count
        OpenDatabase()
        cmd = New MySqlCommand("SELECT featureTable, count FROM genomerunner;", cn)
        dr = cmd.ExecuteReader
        While dr.Read
            localTablesToCounts.Add(dr(0), dr(1))
            lblProgress.Text = "Reading local table: " & dr(0) : Application.DoEvents()
            tableNamesForUCSCQuery.Add("'" & dr(0) & "'")
            'TODO asynchronous?
        End While
        cmd.Dispose()
        dr.Close()

        'establish connection with UCSC database
        Dim ucscCn As MySqlConnection, ucscCmd As MySqlCommand, ucscDr As MySqlDataReader
        ucscCn = New MySqlConnection("Server=genome-mysql.cse.ucsc.edu" & ";Database=" & txtUcscdb.Text & ";User=genomep; Password=password;Connection Timeout=6000;default command timeout=600") : ucscCn.Open()
        ucscCmd = New MySqlCommand("SELECT table_name, TABLE_ROWS FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" & txtUcscdb.Text & "' AND table_name IN (" & Join(tableNamesForUCSCQuery.ToArray, ",") & ");", ucscCn)
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
        ucscCmd.Dispose()
        ucscDr.Close()



        'Now look compare results with genomerunner table.
        For Each tableName In localTablesToCounts.Keys
            'If String.Compare(localTablesToCounts(tableName), ucscTablesToCounts(tableName), True) Then 'comparison will ignore case
            If localTablesToCounts(tableName) <> ucscTablesToCounts(tableName) Then
                'BEWARE: differences in lower/upper case could be an issue here.
                arrayFeaturesAdded.Remove(tableName)
                If (Math.Abs(ucscTablesToCounts(tableName) - localTablesToCounts(tableName)) \ ucscTablesToCounts(tableName)) <= 0.1 Then
                    arrayFeaturesNearlyUpToDate.Add(tableName)
                Else
                    arrayFeaturesToUpdate.Add(tableName)
                End If
            End If
        Next
        cn.Close()
        lblProgress.Text = "Done" : Application.DoEvents()
        GetAddedFeatures()
    End Sub

    Private Sub btnLoadGRTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadGRTable.Click
        If MessageBox.Show("This will drop current genomerunner table. Are you sure you want to continue?", "Confirm", _
                           MessageBoxButtons.OKCancel) = DialogResult.OK Then
            arrayFeaturesToAdd.Clear() 'clears the lsit of feature to add in order to avoid duplicates
            Dim OpenFD As New OpenFileDialog
            OpenFD.Title = "Select a tab delimited .txt file with list of features to be imported to genomerunner table."
            OpenFD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            OpenFD.FileName = ""
            OpenFD.Filter = "All files|*.*|Text Files|*.txt"
            OpenFD.ShowDialog()
            'TODO create genomerunner table if it doesn't exist
            '     truncate it if it does exist.
            fileName = OpenFD.FileName
            If fileName <> "" Then
                CreateGenomeRunnerTable()
                OpenDatabase()
                Using reader As StreamReader = New StreamReader(fileName) 'reads the list of features\
                    reader.ReadLine() 'skip header row
                    While Not reader.EndOfStream
                        Dim line As Array = Split(reader.ReadLine, vbTab)
                        'line could contain null values which will cause an sql error. Insert 'NULL' when this is the case.
                        For i As Integer = 0 To line.Length - 1 Step +1
                            If line(i) = "" Then line(i) = "NULL"
                        Next
                        Dim FeatureTable As String = Trim(line(1))
                        Dim FeatureName As String = Trim(line(2))
                        FeatureName = RemoveIllegalChars(FeatureName)

                        lblProgress.Text = "Loading genomerunner entry for " & FeatureTable : Application.DoEvents()
                        'TODO if entry already exists, get record count. If it matches file, keep all calculated values.
                        cmd = New MySqlCommand("SELECT * FROM genomerunner WHERE FeatureTable = '" & FeatureTable & "';", cn)
                        dr = cmd.ExecuteReader()
                        If dr.HasRows Then
                            'already exists; update existing entry
                            cmd = New MySqlCommand("UPDATE genomerunner SET id=" & line(0) & ", FeatureName='" & FeatureName & "', QueryType='" & line(3) & "', ThresholdType='" & line(4) & "',Tier=" & line(9) & ",Category='" & line(10) & "', orderofcategory=" & line(11) & ", Name='" & line(12) & "' WHERE FeatureTable = '" & FeatureTable & "';", cn)
                        Else
                            'doesn't exist; create new entry
                            'NOTE: this old cmd updates all calculated fields (min, max, etc) as well.
                            'cmd = New MySqlCommand("INSERT INTO genomerunner (id, FeatureTable, FeatureName, QueryType, ThresholdType, ThresholdMin, ThresholdMax, ThresholdMean, ThresholdMedian, Tier, Category, orderofcategory, Name, count, min, max, mean, median, length) VALUES (" & _
                            '                       line(0) & ", '" & line(1) & "', '" & line(2) & "', '" & line(3) & "', '" & line(4) & "', " & line(5) & ", " & line(6) & ", " & line(7) & ", " & line(8) & ", " & line(9) & ", '" & line(10) & "', " & line(11) & ", '" & line(12) & "', " & line(13) & ", " & line(14) & ", " & line(15) & ", " & line(16) & ", " & line(17) & ", " & line(18) & ")", cn)
                            cmd = New MySqlCommand("INSERT INTO genomerunner (id, FeatureTable, FeatureName, QueryType, ThresholdType, Tier, Category, orderofcategory, Name) VALUES (" & _
                                               line(0) & ", '" & Trim(line(1)) & "', '" & FeatureName & "', '" & line(3) & "', '" & line(4) & "', " & line(9) & ", '" & line(10) & "', " & line(11) & ", '" & line(12) & "');", cn)
                            arrayFeaturesToAdd.Add(FeatureTable)
                        End If
                        dr.Close()
                        'arrayFeaturesToAdd.Add(tableName)
                        'Update genomerunner table
                        cmd.ExecuteNonQuery()
                        cmd.Dispose()

                    End While
                End Using
                SyncFeatureToAddListToArrays()
                lblProgress.Text = "Done" : Application.DoEvents()
            End If
        End If
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

    Private Sub btnUpdateStatistics_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateStatistics.Click
        'for each entry in genomerunner, count how many records are in that table and save that to count field of genomerunner
        OpenDatabase()

        For Each tableName In arrayFeaturesAdded
            lblProgress.Text = tableName : Application.DoEvents()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Need to query extra data if this table's genomerunner entry has 'Threshold' as it's QueryType
            Dim thresholdType As String = ""
            Dim thresholdQuery As String = ""
            cmd = New MySqlCommand("SELECT QueryType, ThresholdType FROM genomerunner WHERE FeatureTable = '" & tableName & "';", cn)
            dr = cmd.ExecuteReader
            While dr.Read
                'TODO threshold fields only need calculating when QueryType == "Threshold", right??
                If dr(0) = "Threshold" Then
                    thresholdType = dr(1)
                    thresholdQuery = ", thresholdMin = (SELECT MIN(" & thresholdType & ") from " & tableName & _
                                     "), thresholdMax = (SELECT MAX(" & thresholdType & ") from " & tableName & _
                                     "), thresholdMean = (SELECT AVG(" & thresholdType & ") from " & tableName & ")"
                End If
            End While
            dr.Close()
            cmd.Dispose()
            'TODO update other calulcated fields as well! Most are too complicated to do in a query.
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Update genomerunner
            cmd = New MySqlCommand("UPDATE genomerunner SET count=(SELECT COUNT(*) FROM " & tableName & ")" & thresholdQuery & " WHERE FeatureTable = '" & tableName & "';", cn)
            cmd.ExecuteNonQuery()
            cmd.Dispose()
        Next
        lblProgress.Text = "Done" : Application.DoEvents()
    End Sub

    Private Sub CreateGenomeRunnerTable()
        OpenDatabase()
        cmd = New MySqlCommand("CREATE TABLE IF NOT EXISTS genomerunner (" & _
                                "id INT(11) NOT NULL PRIMARY KEY," & _
                                "FeatureTable VARCHAR(200)," & _
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
                                "length BIGINT(11) unsigned DEFAULT 0" & _
                                ");", cn)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
        cn.Close()
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

        OpenDatabase()
        cmd = New MySqlCommand("SELECT * FROM genomerunner;", cn)
        dr = cmd.ExecuteReader

        Using writer As StreamWriter = New StreamWriter(fileName)
            While dr.Read
                Dim queryAsArray As New ArrayList
                For i As Integer = 0 To dr.FieldCount - 1 Step +1
                    queryAsArray.Add(dr(i).ToString)
                Next
                writer.WriteLine(Join(queryAsArray.ToArray, vbTab))
            End While
        End Using
        lblProgress.Text = "Done" : Application.DoEvents()
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DataDownloadPath = GetSetting("Genome Feature Table Creator", "Data", "DataDownloadPath", AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\")
        lblDataDownloadPath.Text = DataDownloadPath
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
End Class

Class ChromBase
    Public IsHit As Boolean
End Class



