Imports System.IO
Imports Newtonsoft.Json.Linq

Public Class GaxoWorkspace
    Public Property path_ As String
    Dim drawing As Boolean = False
    Dim drawing_started As Boolean = False
    Dim XB As Integer = 0
    Dim YB As Integer = 0
    Dim item_selected

    Sub Unselect_Items_From_ListView(listview)
        For Each item As ListViewItem In listview.SelectedItems
            item.Selected = False
        Next
    End Sub

    Sub AddPropertyRow(table As TableLayoutPanel, propName As String, value As Object)
        Dim rowIndex As Integer = table.RowCount

        ' Increase row count
        table.RowCount += 1
        table.RowStyles.Add(New RowStyle(SizeType.AutoSize))

        ' Property name (label)
        Dim label As New Label()
        label.Text = propName
        label.Dock = DockStyle.Fill
        label.TextAlign = ContentAlignment.MiddleLeft

        ' Property editor (TextBox for now)
        Dim editor As Control
        If TypeOf value Is Boolean Then
            Dim checkBox As New CheckBox()
            checkBox.Checked = CBool(value)
            checkBox.Dock = DockStyle.Left
            editor = checkBox
        Else
            Dim textBox As New TextBox()
            textBox.Text = value.ToString()
            textBox.Dock = DockStyle.Fill
            editor = textBox
        End If

        ' Add to table
        table.Controls.Add(label, 0, rowIndex)
        table.Controls.Add(editor, 1, rowIndex)
    End Sub

    Function Visual_GUI(img, x, y, w, h)
        Dim parent = Panel1
        Dim new_ = New PictureBox()
        new_.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), $"assets/{img}"))
        new_.SizeMode = PictureBoxSizeMode.StretchImage
        new_.Width = w
        new_.BackColor = Color.White
        new_.Height = h
        new_.Location = New Point(x, y)
        new_.Tag = Path.GetFileNameWithoutExtension(img)
        parent.Controls.Add(new_)
        Return new_
    End Function

    Private Sub GaxoWorkspace_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TreeView1.Nodes.Clear()
        Dim rootNode = TreeView1.Nodes.Add(Path.GetFileName(path_))
        rootNode.ImageKey = "folder.png"
        LoadDirectory(path_, rootNode)
        TreeView1.ExpandAll()
        ListView1.Clear()

        For Each folder In Directory.GetDirectories(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "item"))
            Dim type = New ListViewGroup(Path.GetFileName(folder))
            ListView1.Groups.Add(type)
            For Each file_ In Directory.GetFiles(folder)
                Dim jsonText As String = File.ReadAllText(file_)
                Dim obj As JObject = JObject.Parse(jsonText)

                Dim item = New ListViewItem(Path.GetFileNameWithoutExtension(file_))
                item.Group = type
                item.ImageKey = obj("image").ToString()
                ListView1.Items.Add(item)
            Next
        Next
    End Sub

    Private Sub LoadDirectory(dirPath As String, parentNode As TreeNode)
        Try
            ' Add subfolders
            For Each folder In Directory.GetDirectories(dirPath)
                Dim folderNode = parentNode.Nodes.Add(Path.GetFileName(folder))
                folderNode.ImageKey = "folder.png"
                ' Recursive call to load this folder's contents
                LoadDirectory(folder, folderNode)
            Next

            ' Add files
            For Each file In Directory.GetFiles(dirPath)
                Dim fileNode = parentNode.Nodes.Add(Path.GetFileName(file))
                fileNode.ImageKey = "file.png"
            Next
        Catch ex As UnauthorizedAccessException
            ' Skip folders where you don't have access permissions
        End Try
    End Sub

    Private Sub GaxoWorkspace_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Gaxo.Close()
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count > 0 Then

            For Each folder In Directory.GetDirectories(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "item"))
                Dim type = New ListViewGroup(Path.GetFileName(folder))
                ListView1.Groups.Add(type)
                For Each file_ In Directory.GetFiles(folder)
                    If ListView1.SelectedItems(0).Text = Path.GetFileNameWithoutExtension(file_) Then
                        Dim jsonText As String = File.ReadAllText(file_)
                        Dim obj As JObject = JObject.Parse(jsonText)
                        item_selected = obj
                        Panel1.Cursor = Cursors.Cross
                        drawing = True
                        drawing_started = False
                    End If
                Next
            Next

        End If
    End Sub

    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown
        If Not drawing_started And drawing Then
            If item_selected("need_size") Then
                drawing_started = True
                XB = e.X
                YB = e.Y
            Else
                drawing = False
                Panel1.Cursor = Cursors.Default
                XB = e.X
                YB = e.Y
                Dim item = Visual_GUI(item_selected("image"), XB, YB, item_selected("w"), item_selected("h"))
                Unselect_Items_From_ListView(ListView1)
                AddHandler CType(item, PictureBox).Click, AddressOf Item_Click
            End If
        End If
    End Sub

    Private Sub Item_Click(sender As Object, e As EventArgs)
        Dim clickedItem As PictureBox = CType(sender, PictureBox)
        MessageBox.Show("You clicked on image: " & clickedItem.Tag.ToString())
    End Sub

    Private Sub Panel1_MouseUp(sender As Object, e As MouseEventArgs) Handles Panel1.MouseUp
        If drawing_started And drawing Then
            drawing_started = False
            drawing = False
            Panel1.Cursor = Cursors.Default

            Dim item = Visual_GUI(item_selected("image"), Math.Min(XB, e.X), Math.Min(YB, e.Y), Math.Abs(e.X - XB), Math.Abs(e.Y - YB))
            Unselect_Items_From_ListView(ListView1)
            AddHandler CType(item, PictureBox).Click, AddressOf Item_Click
        End If
    End Sub

End Class
