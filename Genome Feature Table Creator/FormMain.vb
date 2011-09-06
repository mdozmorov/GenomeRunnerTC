Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Net
Imports alglib.hqrnd

Public Class FormMain

    Dim listGRFeatures As List(Of Feature) 'used to store feature values that are retrieved from the database
    Dim cn As MySqlConnection, cmd As MySqlCommand, dr As MySqlDataReader, cmd1 As MySqlCommand, dr1 As MySqlDataReader
    Dim featuresSelected As New ArrayList 'stores the features that are selected in the list box
    Dim ftpHost As String = "ftp://hgdownload.cse.ucsc.edu/goldenPath/hg18/database/", ftpUser As String = "anonymous", ftpPassword As String = ""
    Dim uName As String = "", uServer As String = "", uPassword As String = "", uDatabase As String = ""
    Dim arrayFeaturesToAdd As New ArrayList, arrayFeaturesAdded As New ArrayList, arrayFeaturesToRemove As New ArrayList, arrayFeaturesEmpty As New ArrayList, arrayFeaturesNotAddedSuccessfully As New ArrayList 'array of features to be added
    Dim fileName As String, numOfFeaturesToAdd As Integer = 0, progress As Integer = 1 'stores the value of the progress bar
    Dim ConnectionString As String
    Dim Features As New List(Of Feature)
    Dim RandomFeatures As New List(Of Feature)
    Dim backgroundFeatures As New List(Of Feature)
    Dim featureTables As New List(Of String)
    Dim bkgNum As Integer
    'connects to the db
    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        btnExonTable.Enabled = True
        btnGenerateBackground.Enabled = True
        listFeaturesToAdd.Items.Clear() 'clears the list
        OpenDatabase()
        GetFeaturesAvailableList() 'gets list of features that can be added
        GetAddedFeatures() 'loads the table names of features added to list
        GetEmptyFeatures() 'checks for features in the db that are empty
        syncGenomeTableToDatabase() 'checks for features that have been added to the genomerunner table but not actually added to the database
    End Sub

    'opens a connection to the database; uses values supplied by the user
    Private Sub OpenDatabase()
        Dim uName As String = txtUser.Text
        Dim uServer As String = txtHost.Text
        Dim uPassword As String = txtPassword.Text
        Dim uDatabase As String = txtDatabase.Text
        ' If uName <> "" And uPassword <> "" And uServer <> "" And uDatabase <> "" Then
        ConnectionString = "Server=" & uServer & ";Database=" & uDatabase & ";User ID=" & uName & ";Password=" & uPassword & ";Connection Timeout=6000;" 'uses the values provided by user to create constring. TODO add error checking
        ' Else
        ' ConnectionString = "Server=VM-Wren-01;Database=hg18;User ID=Genome;Password=Runner"
        ' End If
        'If IsNothing(cn) Then
        Try
            If IsNothing(cn) Then
                cn = New MySqlConnection(ConnectionString) : cn.Open()
            End If
            If cn.State = ConnectionState.Closed Then
                cn = New MySqlConnection(ConnectionString) : cn.Open()
            End If
            'End If
            ' If cn.State = ConnectionState.Closed Then
            ' cn = New MySqlConnection(ConnectionString) : cn.Open()
            ' End If
        Catch
            MessageBox.Show("Please check connection settings and try again")
        End Try
    End Sub

    'gets the feature list that is saved locally 
    Private Sub GetFeaturesAvailableList()
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
        listFeaturesToAdd.Items.Clear()
        Try
            cmd = New MySqlCommand("SELECT featuretable FROM genomerunner", cn)
            Dim dr As MySqlDataReader
            dr = cmd.ExecuteReader()
            While dr.Read()
                arrayFeaturesAdded.Add(dr(0))
                listFeaturesToAdd.Items.Add(arrayFeaturesAdded(arrayFeaturesAdded.Count - 1))
            End While
            cn.Close() : dr.Close()
        Catch
        End Try
    End Sub
    'get empty feature tables
    Private Sub GetEmptyFeatures()
        OpenDatabase()
        arrayFeaturesEmpty.Clear()
        For x As Integer = 0 To listFeaturesToAdd.Items.Count Step +1
            Try
                cmd = New MySqlCommand("SELECT * FROM " & listFeaturesToAdd.Items(x) & " LIMIT 1", cn)
                dr = cmd.ExecuteReader()
                If dr.HasRows = False Then
                    arrayFeaturesEmpty.Add(listFeaturesToAdd.Items(x))
                End If
                dr.Close()
            Catch

            End Try
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
            If File.Exists(AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\" & feature & ".sql") = False Then
                lblProgress.Text = "Downloading " & feature & ".sql" : Application.DoEvents()
                My.Computer.Network.DownloadFile("ftp://hgdownload.cse.ucsc.edu/goldenPath/hg18/database/" & feature & ".sql", AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\" & feature & ".sql") 'downloads the .sql feature file
            End If
            If File.Exists(AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\" & feature & ".txt.gz") = False Then
                lblProgress.Text = "Downloading " & feature & ".txt.gz" : Application.DoEvents()
                My.Computer.Network.DownloadFile("ftp://hgdownload.cse.ucsc.edu/goldenPath/hg18/database/" & feature & ".txt.gz", AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\" & feature & ".txt.gz") 'downloads the .txt.gz feature file

            End If
            'decompresses the .gz file to a .txt file
            If File.Exists(AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\" & feature & ".txt") = False Then
                lblProgress.Text = "Decompressing " & feature : Application.DoEvents()
                Using outfile As FileStream = File.Create(AppDomain.CurrentDomain.BaseDirectory.ToString() & "FeaturesToAdd\" & feature & ".txt")
                    Using infile As FileStream = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory.ToString() & "FeaturesToAdd\" & feature & ".txt.gz")
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
        Dim filePathFeaturesToAdd = AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\"
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
        Dim filePathFeaturesToAdd = AppDomain.CurrentDomain.BaseDirectory & "FeaturesToAdd\"
        Dim filePathFeatureToAddUNIX As String = filePathFeaturesToAdd.Replace("\", "/") 'modifies the filepath to conform to UNIX 
        Dim query As String = "LOAD DATA LOCAL INFILE " & "'" & filePathFeatureToAddUNIX & feature & ".txt' INTO TABLE " & feature
        Try
            cmd = New MySqlCommand(query, cn) 'reads the .txt file and inserts the data into the created table
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            Dim form As New FormQuery
            form.txtQuery.Text = query
            form.ShowDialog()

        End Try
        cn.Close()
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
        e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault)
        If arrayFeaturesToAdd.IndexOf(listFeaturesToAdd.Items.Item(e.Index)) <> -1 Then 'searches if the feature in the list of features in the db is in the array of feastures to add
            e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, Brushes.BlueViolet, e.Bounds, StringFormat.GenericDefault) 'paints the text green
        ElseIf arrayFeaturesToRemove.IndexOf(listFeaturesToAdd.Items(e.Index).ToString()) <> -1 Then 'searches if the feature in the list of features from thd db is in the list of feature to remove
            e.Graphics.DrawString(listFeaturesToAdd.Items(e.Index).ToString(), e.Font, Brushes.Red, e.Bounds, StringFormat.GenericDefault) 'paints the text red
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
        Try
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
        Catch
        End Try
    End Sub

    Private Sub btnPrepareFiles_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrepareFiles.Click
        ProgressBar1.Maximum = arrayFeaturesToAdd.Count
        ProgressBar1.Value = 0
        For x As Integer = 0 To arrayFeaturesToAdd.Count - 1 Step +1
            PrepareFeatures(arrayFeaturesToAdd(x))
            ProgressBar1.Value += 1
        Next
        lblProgress.Text = "Done"
        ProgressBar1.Value = 0
        syncGenomeTableToDatabase()
    End Sub

    Private Sub btnCreateTables_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateTables.Click
        ProgressBar1.Maximum = arrayFeaturesToAdd.Count
        ProgressBar1.Value = 0
        Dim arrayAddedSuccessfully As New ArrayList
        For x As Integer = 0 To arrayFeaturesToAdd.Count - 1 Step +1
            AddFeaturesToDatabase(arrayFeaturesToAdd(x))  'runs the .sql query 
            ProgressBar1.Value += 1
        Next
        arrayFeaturesToAdd.Clear() 'clears the array of features to add
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
        For x As Integer = 0 To arrayFeaturesEmpty.Count - 1 Step +1
            lblProgress.Text = "Loading data for " & arrayFeaturesEmpty(x) : Application.DoEvents()
            PopulateDatabse(arrayFeaturesEmpty(x))
            ProgressBar1.Value += 1
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
            SetGenomeBackgroundHG18()
            SetRandFeatureInterval1(Features.Count - 1)
            AddRandomToDatabase()
            lblProgress.Text = "Done"
            MessageBox.Show("Random feature generated")
        Else
            MessageBox.Show("Please Select a feature from which to generate the random background from the list of features")
        End If
    End Sub

    Public Sub SetGenomeBackgroundHG18()
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
            BackgroundFeatures.Add(feature)
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
End Class

Class ChromBase
    Public IsHit As Boolean
End Class

