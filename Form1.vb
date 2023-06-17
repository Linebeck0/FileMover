Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim base As String = TextBox1.Text
        Dim seperator As String = TextBox2.Text
        doWork(base:=base, seperator:=seperator)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If BackgroundWorker1.IsBusy Then
            BackgroundWorker1.CancelAsync()
            Button2.Text = "Move Files (Loop)"
        Else
            BackgroundWorker1.RunWorkerAsync()
            Button2.Text = "Move Files (Stop)"
        End If
        Button1.Visible = Not Button1.Visible
        Label3.Visible = Not Label3.Visible
        ProgressBar1.Visible = Label3.Visible
    End Sub
    Private Sub doWork(base As String, seperator As String)
        For Each file As String In My.Computer.FileSystem.GetFiles(base)
            Dim myFile As New System.IO.FileInfo(file)
            If myFile.Length = 0 OrElse myFile.LastWriteTime > Now.AddMinutes(-60) Then
                Continue For
            End If
            If file.Contains(seperator) Then
                Dim filename As String = file.ToLower
                Label3.Invoke(Sub() Label3.Text = file)
                filename = filename.Substring(filename.IndexOf(seperator) + 1)
                Dim yourpath As String = base + "\" + filename
                Dim targetfile As String = yourpath + file.Substring(file.LastIndexOf("\"))
                If Not System.IO.Directory.Exists(yourpath) Then
                    System.IO.Directory.CreateDirectory(yourpath)
                End If
                Try
                    My.Computer.FileSystem.MoveFile(file, targetfile)
                    System.Threading.Thread.Sleep(file.Substring(file.LastIndexOf("\")).Length * 25)
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
            End If
        Next
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim base As String = TextBox1.Text
        Dim seperator As String = TextBox2.Text
        Try
            While Not BackgroundWorker1.CancellationPending()
                doWork(base:=base, seperator:=seperator)
                ProgressBar1.Invoke(Sub() ProgressBar1.Value = ProgressBar1.Maximum)
                While ProgressBar1.Value <> ProgressBar1.Minimum
                    If Not BackgroundWorker1.CancellationPending Then
                        DecreaseProgressBar()
                        System.Threading.Thread.Sleep(1000)
                    Else
                        ProgressBar1.Invoke(Sub() ProgressBar1.Value = ProgressBar1.Minimum)
                        Label3.Invoke(Sub() Label3.Text = "Stopped")
                        Exit While
                    End If
                End While
            End While
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Debugger.IsAttached Then
            TextBox1.Text = "\\GVM05\yt"
        End If
    End Sub

    Private Sub DecreaseProgressBar()
        If ProgressBar1.Value <> ProgressBar1.Minimum Then
            ProgressBar1.Invoke(Sub() ProgressBar1.Value -= 1)
            Label3.Invoke(Sub() Label3.Text = "Waiting ... " & ProgressBar1.Value.ToString)
        End If
    End Sub
    Private Sub ProgressBar1_Click(sender As Object, e As EventArgs) Handles ProgressBar1.Click
        DecreaseProgressBar()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Using dialog As New FolderBrowserDialog()
            If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                TextBox1.Text = dialog.SelectedPath
            End If
        End Using
    End Sub
End Class
