Imports System.IO

Public Class Gaxo
    Dim full_path
    Private Sub quit_btn_Click(sender As Object, e As EventArgs) Handles quit_btn.Click
        Application.Exit()
    End Sub

    Private Sub New_project_Click(sender As Object, e As EventArgs) Handles New_project.Click
        GaxoNewProject.full_path = full_path
        GaxoNewProject.ShowDialog()
    End Sub

    Private Sub Gaxo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim documentsPath As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        full_path = Path.Combine(documentsPath, "Gaxo Projects")

        If Not Directory.Exists(full_path) Then
            Directory.CreateDirectory(full_path)
        End If
    End Sub

    Private Sub load_project_Click(sender As Object, e As EventArgs) Handles load_project.Click
        Using folderDialog As New FolderBrowserDialog()
            folderDialog.Description = "Select a project folder"
            folderDialog.SelectedPath = full_path
            folderDialog.ShowNewFolderButton = False

            If folderDialog.ShowDialog() = DialogResult.OK Then
                Dim selectedPath As String = folderDialog.SelectedPath

                GaxoWorkspace.path_ = selectedPath
                GaxoWorkspace.Show()
                Me.Hide()

            End If
        End Using
    End Sub

End Class
