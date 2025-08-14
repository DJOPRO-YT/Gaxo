Imports System.IO

Public Class GaxoNewProject
    Public Property full_path As String
    Dim path_ As String
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        path_ = Path.Combine(full_path, TextBox1.Text)
        Directory.CreateDirectory(path_)
        Dim TextToStore As String = $"{{""type"":""{ComboBox1.Text}"",""os_version"":""{TextBox3.Text}""}}"
        File.WriteAllText(Path.Combine(path_, "config.json"), TextToStore)

        Directory.CreateDirectory(Path.Combine(path_, "Bin"))
        TextToStore = "{""access"": ["
        For Each item In CheckedListBox1.CheckedItems
            TextToStore = TextToStore + """" + item.ToString() + ""","
        Next

        If TextToStore.Substring(TextToStore.Length - 1, 1) = "," Then
            TextToStore = TextToStore.Substring(0, TextToStore.Length - 1)
        End If

        TextToStore = TextToStore + "],""name"":""" + TextBox2.Text + """,""os_version"":""" + TextBox3.Text + """}"
        File.WriteAllText(Path.Combine(path_, "Bin/manifest.json"), TextToStore)

        File.WriteAllText(Path.Combine(path_, "Bin/Design.lua"), "local design = {}\n\nreturn design")
        File.WriteAllText(Path.Combine(path_, "Bin/Code.lua"), "local design = require(""./Design.lua"")")


        GaxoWorkspace.path_ = path_
        GaxoWorkspace.Show()
        Gaxo.Hide()
        Me.Close()
    End Sub
End Class