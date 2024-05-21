﻿Public Class Barcode
    Dim gbarcode As New MessagingToolkit.Barcode.BarcodeEncoder
    Dim barcodeimage As Image
    Private Sub Barcode_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label1.Text = "Enter the Product  :"
        PictureBox1.Visible = False
        load()
    End Sub


    Private Sub ButtonColumn_Click(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        Dim clickedRowIndex As Integer = e.RowIndex
        Dim barcodee = DataGridView1.CurrentRow.Cells(4).Value()
        PictureBox1.Image = generate(barcodee)
        barcodeimage = New Bitmap(gbarcode.Encode(MessagingToolkit.Barcode.BarcodeFormat.Code128, barcodee.ToString))
        Dim print As New PrintDialog()
        print.Document = PrintDocument1
        If print.ShowDialog() = DialogResult.OK Then
            PrintDocument1.PrinterSettings.Copies = print.PrinterSettings.Copies
            PrintDocument1.Print()
        End If


    End Sub
    Private Sub DataGridView1_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        DataGridView1.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Outset
        DataGridView1.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Outset
        DataGridView1.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.Inset
        If e.RowIndex Mod 2 = 0 Then

            e.CellStyle.BackColor = Color.LightBlue
        Else
            e.CellStyle.BackColor = Color.GhostWhite
        End If
    End Sub


    Private Sub PrintDocument1_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Dim height2 = 0
        'e.Graphics.DrawImage(PictureBox1.Image, 4, 4)
        e.Graphics.DrawImage(barcodeimage, CInt((e.PageBounds.Width - 80) / 2), 10 + height2 + 25, 100, 35)
    End Sub



    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        load()
    End Sub

    Public Sub autocomplete()
        Dim query As String = "Select product_id as 'Product Id', product_name , Category.Category as Category , brands.brand as 'Brand Name',Barcode from products INNER JOIN Category ON products.cat_id = Category.cat_id INNER JOIN brands ON brands.brand_id = products.brand_id WHERE 1=1 and products.product_name Like '%" & TextBox1.Text.Replace("'", "''") & "%' and products.status='1'"
        Dim dataTable As DataTable = LoadDataTable(query)

        If dataTable IsNot Nothing Then

            Dim col As New AutoCompleteStringCollection
            For Each row As DataRow In dataTable.Rows
                col.Add(row("product_name").ToString)
            Next
            TextBox1.AutoCompleteSource = AutoCompleteSource.CustomSource
            TextBox1.AutoCompleteCustomSource = col
            TextBox1.AutoCompleteMode = AutoCompleteMode.Suggest
        End If

    End Sub

    Public Sub load()
        Dim query As String = "Select product_id as 'Product Id', product_name as 'Product Name', Category.Category as Category , brands.brand as 'Brand Name',Barcode from products INNER JOIN Category ON products.cat_id = Category.cat_id INNER JOIN brands ON brands.brand_id = products.brand_id WHERE 1=1 and products.product_name Like '%" & TextBox1.Text.Replace("'", "''") & "%' and products.status='1'"
        Dim dataTable As DataTable = LoadDataTable(query)
        DataGridView1.DataSource = dataTable
        DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.Black
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Black
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.Columns(0).Width = 100
        DataGridView1.Columns(1).Width = 175
        DataGridView1.Columns(2).Width = 100
        DataGridView1.Columns(3).Width = 150
        DataGridView1.Columns(4).Width = 110




    End Sub



End Class