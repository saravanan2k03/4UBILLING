﻿
Imports System.Data.SqlClient
Imports System.Drawing.Printing
Imports PdfSharp.Pdf

Public Class BILLING
    Public allow = 0
    Public ProductId As String
    Public ReduceAmount As Double = 0
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Size = New Size(1395, 569)
        Me.BarcodeCodetxt.Focus()
        LoadAutoComplete()
        GeneratetheBillNo()
        InitialLoad()
        Me.Quantity.Text = 1
        Dim validate As New DataGridViewButtonColumn()
        Dim delete As New DataGridViewButtonColumn()
        With validate
            .Name = "val"
            .HeaderText = "UPDATE"
            .Text = "UPDATE"
            .UseColumnTextForButtonValue = True
            .CellTemplate = New DataGridViewButtonCell()
        End With
        With delete
            .Name = "del"
            .HeaderText = "DELETE"
            .Text = "DELETE"
            .CellTemplate = New DataGridViewButtonCell()
        End With
        validate.UseColumnTextForButtonValue = True
        delete.UseColumnTextForButtonValue = True
        BillingGridsumma.Columns.Add(validate)
        BillingGridsumma.Columns.Add(delete)
        BillingGridsumma.ColumnHeadersDefaultCellStyle.BackColor = Color.Black
        BillingGridsumma.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
    End Sub



    Private Sub InitialLoad()
        BarcodeCodetxt.Focus()
        Me.ProductName.Clear()
        Me.ReturnAmount.Clear()
        Me.Return_billno.Clear()
        Me.Amount.Clear()
        Me.Balance.Clear()
        Me.Total.Text = 0
        Me.Price.Text = 0
        Me.MobileNo.Text = ""
        Me.BarcodeCodetxt.Clear()
        Me.DiscountIo.Clear()
        Me.DiscountOut.Clear()
        Me.Amount.Clear()
        Me.Balance.Clear()
        LoadGrid(Me.Bill_no.Text)
        BarcodeCodetxt.Focus()
    End Sub

    Private Sub GettheProduct(Barcodeval As String)
        Dim Query As String = "select pro.Product_id As 'PRODUCT ID',Pro.Product_name As 'PRODUCT NAME',pro.Quantity As 'QUANTITY',pro.price As 'PRICE' from dbo.Products As pro inner join Category As cat on cat.Cat_id = pro.Cat_id inner join Brands As Brand on Brand.Brand_id = pro.Brand_id where pro.Barcode = @Barcode And pro.Status = 1 And cat.Status = 1 And Brand.Status =1"
        Dim con As SqlConnection = New SqlConnection(connectionString)
        Try
            Using command As New SqlCommand(Query, con)
                command.Parameters.AddWithValue("@Barcode", Barcodeval)
                con.Open()
                ' Execute the query
                Using reader As SqlDataReader = command.ExecuteReader()
                    ' Check if there are any rows returned
                    If reader.HasRows Then
                        While reader.Read()
                            Dim ProductName As String = reader("PRODUCT NAME").ToString
                            'Dim Quantity As String = reader("QUANTITY").ToString
                            Dim Price As String = reader("PRICE").ToString
                            ProductId = reader("PRODUCT ID").ToString
                            Me.ProductName.Text = ProductName
                            Me.Price.Text = Price
                            Me.Quantity.Focus()
                            'MessageBox.Show($"Product Name: {ProductName}{Environment.NewLine}Quantity: {Quantity}{Environment.NewLine}Price: {Price}", "Product Details", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            IncrementTheTotal(Me.Quantity.Text, Me.Price.Text)

                        End While
                    Else
                        ' No rows found - display an error message
                        MessageBox.Show("Invalid barcode. Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        BarcodeCodetxt.Clear()
                        BarcodeCodetxt.Focus()
                        InitialLoad()
                        Me.Quantity.Text = 1
                        BarcodeCodetxt.Focus()
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Handle any exceptions that may occur
            MessageBox.Show($"An error occurred get the product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close()
        End Try
    End Sub


    Private Sub IncrementTheTotal(Quantity As String, Price As String)
        Try
            Dim Quan As Int32 = Convert.ToInt32(Quantity)
            Dim pri As Double = Convert.ToDouble(Price)

            If Quan >= 0 Then
                Dim sum As Double = Quan * pri
                Me.Total.Text = sum.ToString("0.00")
            Else
                MsgBox("Provide an Appropriate Quantity")
                Me.Quantity.Clear()
                Me.Quantity.Text = 0
                Me.Quantity.Focus()
            End If
        Catch ex As Exception
            Me.Total.Text = 0
        End Try
    End Sub

    Private Sub AddProduct(Barcodeval As String, BillNo As String)
        If Convert.ToDouble(Me.Total.Text) > 0 Then
            Dim Query As String = "select Products.Product_id As 'PRODUCT ID',Product_Name As 'PRODUCT NAME',bill.Quantity As 'QUANTITY',bill.Price As 'PRICE',bill.Total As 'TOTAL',bill.GrandTotal As 'GRAND TOTAL' from dbo.Products inner join dbo.Billing As bill on bill.Product_id = Products.Product_id where Products.Barcode =@Barcode And bill.Billing_no=@BillNo And bill.Status = 0 "
            Dim con As SqlConnection = New SqlConnection(connectionString)
            Try
                Using command As New SqlCommand(Query, con)
                    command.Parameters.AddWithValue("@Barcode", Barcodeval)
                    command.Parameters.AddWithValue("@BillNo", BillNo)
                    con.Open()
                    ' Execute the query
                    Using reader As SqlDataReader = command.ExecuteReader()
                        ' Check if there are any rows returned
                        If reader.HasRows Then
                            While reader.Read()
                                Dim Quantity As Int32 = Convert.ToInt32(reader("QUANTITY"))
                                Dim Price As Double = Convert.ToDouble(reader("PRICE"))
                                Dim Total As String = reader("TOTAL")
                                Quantity += Convert.ToInt32(Me.Quantity.Text)
                                Total = Quantity * Price
                                Dim QuantityUpdateQuery As String = "update Billing set Quantity =@Quantity, Total=@Total where Billing_no =@BillNo and Status =0 and Barcode=@Barcodeval"
                                Dim parameter As New List(Of SqlParameter)
                                parameter.Add(New SqlParameter("@Quantity", Quantity))
                                parameter.Add(New SqlParameter("@Total", Total))
                                parameter.Add(New SqlParameter("@BillNo", BillNo))
                                parameter.Add(New SqlParameter("@Barcodeval", Barcodeval))
                                Dim QuantityCheckData As Int32 = QuantityCheck(Convert.ToInt32(reader("PRODUCT ID")), Quantity)
                                If QuantityCheckData = 1 Then
                                    QueryProcess(QuantityUpdateQuery, parameter)
                                    BarcodeCodetxt.Focus()
                                Else
                                    MsgBox("Add Product In Inventory")
                                End If
                            End While
                        Else
                            Dim Query1 As String = "select * from Products where Barcode =@Barcodeval"
                            Dim con1 As SqlConnection = New SqlConnection(connectionString)

                            Using command1 As New SqlCommand(Query1, con1)
                                command1.Parameters.AddWithValue("@Barcodeval", Barcodeval)
                                con1.Open()
                                Using reader1 As SqlDataReader = command1.ExecuteReader()
                                    If reader1.HasRows Then
                                        Dim InsertQuery As String = "Insert into Billing (Billing_no,Product_id,Quantity,Price,Total,Barcode,Billed_by,Date,Status,GrandTotal) values(@Billingno,@ProductId,@Quantity,@Price,@Total,@Barcode,@Billedby,GETDATE(),0,@Grandtotal)"
                                        Dim InsertParameter As New List(Of SqlParameter)
                                        InsertParameter.Add(New SqlParameter("@Billingno", BillNo))
                                        InsertParameter.Add(New SqlParameter("@ProductId", Convert.ToInt32(ProductId)))
                                        InsertParameter.Add(New SqlParameter("@Quantity", Convert.ToInt32(Me.Quantity.Text)))
                                        InsertParameter.Add(New SqlParameter("@Price", Me.Price.Text))
                                        InsertParameter.Add(New SqlParameter("@Total", Me.Total.Text))
                                        InsertParameter.Add(New SqlParameter("@Barcode", Me.BarcodeCodetxt.Text))
                                        If (Len(userID)) <= 0 Then
                                            MsgBox("UserIdEmpty")
                                            userID = "1"
                                        End If
                                        InsertParameter.Add(New SqlParameter("@Billedby", userID))
                                        InsertParameter.Add(New SqlParameter("@Grandtotal", "0"))
                                        Dim QuantityCheckDataelse As Int32 = QuantityCheck(Convert.ToInt32(ProductId), Convert.ToInt32(Me.Quantity.Text))
                                        If QuantityCheckDataelse = 1 Then
                                            InsertDataas(InsertQuery, InsertParameter)
                                            BarcodeCodetxt.Focus()
                                        Else
                                            MsgBox("Add Product In Inventory")
                                        End If
                                    Else
                                        MessageBox.Show("Invalid barcode. Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        BarcodeCodetxt.Clear()
                                        BarcodeCodetxt.Focus()
                                        InitialLoad()
                                        Me.Quantity.Text = 1
                                    End If
                                End Using

                            End Using


                        End If
                    End Using
                End Using
            Catch ex As Exception
                ' Handle any exceptions that may occur
                MessageBox.Show($"An error occurred add product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                con.Close()

            End Try
        Else
            MsgBox("Please Add The Product!")
        End If

    End Sub


    Private Sub UpdateProduct(Ref_id As String, Quantity As String, Price As String)
        Dim a As Integer
        Dim t As String
        Dim newQuantity As String
        Dim Total As String
        newQuantity = InputBox("Enter the Quantity", "UPDATE")

        ' After loop, newQuantity is either a valid number or empty string
        If newQuantity <> "" And IsNumeric(newQuantity) Then
            ' If newQuantity is a valid number, perform calculations

            Total = CInt(newQuantity) * Convert.ToDouble(Price)
            t = "Please Confirm the details ..."
            t = t & vbCrLf & vbCrLf & "Quantity : " & newQuantity
            t = t & vbCrLf & "Price : " & Price
            t = t & vbCrLf & "Total : " & Total
            a = MsgBox(t, vbOKCancel, "Please Confirm ...")
            If a = vbOK And newQuantity > 0 Then
                Dim updateQuery As String = "update dbo.Billing set Quantity=@Quantity,Total=@Total where ref_id =@refid"
                Dim updateParameter As New List(Of SqlParameter)
                updateParameter.Add(New SqlParameter("@Quantity", Convert.ToInt32(newQuantity)))
                updateParameter.Add(New SqlParameter("@Total", Total))
                updateParameter.Add(New SqlParameter("@refid", Convert.ToInt32(Ref_id)))
                Dim GetProductId As Int32 = GetProductIdUsingRefId(Convert.ToInt32(Ref_id))
                If GetProductId <> -1 Then
                    Dim QuantityCheckDataelse As Int32 = QuantityCheck(GetProductId, Convert.ToInt32(newQuantity))
                    If QuantityCheckDataelse = 1 Then

                        If QueryProcess(updateQuery, updateParameter) = 1 Then
                            LoadGrid(Me.Bill_no.Text)
                            MsgBox("Product Updated!")
                        End If

                    Else
                        MsgBox("Add Product In Inventory")
                    End If
                End If

            End If
        Else
            Exit Sub
        End If
    End Sub

    Private Sub ReduceReturnAmount()
        If Convert.ToInt32(GetTheReturnAmount(Me.Return_billno.Text)) > 0 Then
            ReduceAmount = GetTheReturnAmount(Me.Return_billno.Text)
        Else
            MsgBox("please Enter Valid BillNo")
            Me.ReturnAmount.Clear()
            Me.Return_billno.Clear()
        End If
    End Sub

    Private Function DeleteProduct(Sqlquery As String, Parameters As List(Of SqlParameter))
        Try
            Using con As SqlConnection = New SqlConnection(connectionString)
                con.Open()
                Using command1 As New SqlCommand(Sqlquery, con)
                    ' Add parameters to the SqlCommand
                    For Each parameter As SqlParameter In Parameters
                        command1.Parameters.Add(parameter)
                    Next
                    command1.ExecuteNonQuery()
                End Using
            End Using
            ' If execution completes without errors, return 1
            Return 1
        Catch ex As Exception
            ' Log the exception or handle it appropriately
            Debug.WriteLine("An error occurred: " & ex.Message)
            ' If an error occurs, return -1
            Return -1
        End Try
    End Function
    Private Sub LoadGrid(BillNo As String)
        Try
            Dim query As String = "select ref_id As 'REF ID',pro.Product_name As 'PRODUCT NAME',cat.Category As 'CATEGORY',Brand.Brand As 'BRAND',Bill.Quantity As 'QUANTITY',Bill.Price As 'PRICE',Bill.Total 'TOTAL' from dbo.Billing As Bill inner join Products As pro on pro.Product_id = Bill.Product_id  inner join Category As cat on cat.Cat_id = pro.Cat_id inner join Brands As Brand on Brand.Brand_id = pro.Brand_id where Bill.Status = 0 And Bill.Billing_no = @BillNO"
            Dim parameters As New List(Of SqlParameter)
            parameters.Add(New SqlParameter("@BillNO", BillNo))
            Dim gridSizes As New List(Of Double)
            If allow = 0 Then
                allow = 1
                gridSizes.AddRange({100, 156, 100, 100, 120, 100, 100})
            Else
                gridSizes.AddRange({100, 100, 100, 156, 100, 100, 120, 100, 100})
            End If

            Dim objgrid As New GridClass
            objgrid.gridWithPram(BillingGridsumma, query, {0, 1, 2, 3, 4, 5, 6}.ToList, gridSizes, parameters)

            BillingGridsumma.ColumnHeadersDefaultCellStyle.BackColor = Color.Black
            BillingGridsumma.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            'CalCulate GrandTotal
            CalculateGrandTotal(BillNo)
        Catch ex As Exception
            MsgBox($"SQL Exception occurred loadgrid: {ex.Message}", MsgBoxStyle.Critical, "SQL Error")
            Debug.WriteLine(ex.ToString)
        End Try


    End Sub
    Private Sub CalculateGrandTotal(BillNo As String)
        Dim Query As String = "select sum(convert(float,Total)) As 'Total'  from Billing where Billing_no =@BillNo"
        Dim con As SqlConnection = New SqlConnection(connectionString)
        Try
            Using command As New SqlCommand(Query, con)
                command.Parameters.AddWithValue("@BillNo", BillNo)
                con.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    If reader.HasRows Then
                        While reader.Read()
                            If reader("Total").ToString = "" Then
                                Me.grandtot.Text = "0"
                            Else
                                Dim totalValue As Double = Convert.ToDouble(reader("Total"))
                                Dim roundedTotal As Double = Math.Round(totalValue)
                                Dim roundedTotalAsString As String = roundedTotal.ToString()
                                Me.grandtot.Text = totalValue.ToString("0.00")
                            End If
                        End While
                    End If
                End Using

            End Using
            Final_amount_val(Me.ReturnAmount.Text, Me.DiscountOut.Text, Me.grandtot.Text)

            ' Execute the query
        Catch ex As Exception
            MsgBox($"SQL Exception occurred CalculateGrandTotal: {ex.Message}", MsgBoxStyle.Critical, "SQL Error")
        Finally
            con.Close()
        End Try

    End Sub


    Private Sub GeneratetheBillNo()
        Dim random As New Random()
        Dim characters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
        Dim randomString As String = ""

        For i As Integer = 1 To 6
            Dim randomIndex As Integer = random.Next(0, characters.Length)
            randomString += characters(randomIndex)
        Next
        Me.Bill_no.Text = randomString
    End Sub

    Private Sub FinalizeBill()
        If HasDataCheck(Me.Bill_no.Text) = 1 Then

            If Len(Me.MobileNo.Text) <> 0 Then
                Dim Query As String = "Select Customer_id,CustomerName,MobileNo,Place from Customer where MobileNo=@Mobileno"
                Dim con As SqlConnection = New SqlConnection(connectionString)
                Try
                    Using command As New SqlCommand(Query, con)
                        command.Parameters.AddWithValue("@Mobileno", Me.MobileNo.Text)
                        con.Open()
                        Using reader As SqlDataReader = command.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    Dim CustomerId As Int32 = Convert.ToInt32(reader("Customer_id"))
                                    Dim disper As Double = 0
                                    If Convert.ToInt32(DiscountIo.Text) < 100 And DiscountIo.Text <> 0 Then
                                        disper = Convert.ToDouble(DiscountIo.Text)
                                    End If
                                    Dim InsertCusidQuery As String = "update Billing set Customer_id =@CustomerID,Status=1,GrandTotal=@GrandTotal,DiscountPercentage=@DiscountPercentage,Billed_by=@Billedby where Billing_no =@BillNO"
                                    Dim parameters As New List(Of SqlParameter)
                                    parameters.Add(New SqlParameter("@BillNO", Me.Bill_no.Text))
                                    parameters.Add(New SqlParameter("@CustomerID", CustomerId))
                                    parameters.Add(New SqlParameter("@GrandTotal", Me.grandtot.Text))
                                    parameters.Add(New SqlParameter("@Billedby", 1))
                                    parameters.Add(New SqlParameter("@DiscountPercentage", disper))
                                    Dim ReduceQuantity As Int32 = FinalizeBillingForReduceQuantity(Me.Bill_no.Text)
                                    If ReduceQuantity = 1 Then
                                        Dim Exec As Int32 = QueryProcess(InsertCusidQuery, parameters)
                                        If Exec = 1 Then
                                            Try
                                                GenerateBill()
                                            Catch ex As Exception
                                                MsgBox(ex.ToString)
                                            End Try
                                            GeneratetheBillNo()
                                            InitialLoad()
                                            Me.Quantity.Text = 1
                                        End If
                                    End If
                                End While
                            Else
                                MessageBox.Show($"An error occurred User Mobile Number: {Me.MobileNo.Text}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        End Using

                    End Using
                    ' Execute the query
                Catch ex As Exception
                    MsgBox($"SQL Exception occurred FinalizeBill: {ex.Message}", MsgBoxStyle.Critical, "SQL Error")
                Finally
                    con.Close()
                End Try
            Else
                MessageBox.Show($"Please provide mobile number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Me.MobileNo.Focus()
            End If
        Else
            MessageBox.Show($"Please Buy Product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.BarcodeCodetxt.Focus()
        End If
    End Sub

    Private Function GenerateBill()
        Try
            changelongpaper()

            Dim printDialog As New PrintDialog()
            printDialog.Document = PD
            Dim myPrinters = PrinterSettings.InstalledPrinters.OfType(Of String).
                     Where(Function(p) p.Contains("POS-80C")).ToList()
            If Not myPrinters.Count > 0 Then
                MsgBox("printer Not Found!")
                Return -1
            Else
                Dim printerName As String = myPrinters(0)
                'MsgBox("Printer found: " & printerName)
                ' Set the printer for PrintDocument
                PD.PrinterSettings.PrinterName = printerName
                PD.PrinterSettings.Copies = 2
                PD.Print()
                Return 1
            End If
        Catch ex As Exception
            Return -1
        End Try

    End Function


    Dim WithEvents PD As New PrintDocument
    'Dim PPD As New PrintPreviewDialog
    Dim longpaper As Integer
    Sub changelongpaper()
        Dim rowcount As Integer
        longpaper = 0
        rowcount = BillingGridsumma.Rows.Count
        longpaper = rowcount * 70 + 300
        longpaper = longpaper + 230
    End Sub


    Private Sub PD_BeginPrint(sender As Object, e As PrintEventArgs) Handles PD.BeginPrint
        Dim pagesetup As New PageSettings
        'pagesetup.PaperSize = New PaperSize("Custom", 250, 500) 'fixed size
        pagesetup.PaperSize = New PaperSize("Custom", 250, longpaper)
        'pagesetup.Margins = New Margins(2, 2, 2, 50)
        PD.DefaultPageSettings = pagesetup

    End Sub

    Private Sub PD_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PD.PrintPage
        Dim d As String = DateTime.Now.ToString("dd-MM-yyyy")
        Dim stat As String = TimeOfDay.ToString("h:mm:ss tt")
        Dim f8 As New Font("Calibri", 8, FontStyle.Regular)
        Dim f10 As New Font("Calibri", 10, FontStyle.Regular)
        Dim f10b As New Font("Calibri", 10, FontStyle.Bold)
        Dim f14 As New Font("Calibri", 14, FontStyle.Bold)

        Dim leftmargin As Integer = PD.DefaultPageSettings.Margins.Left
        Dim centermargin As Integer = PD.DefaultPageSettings.PaperSize.Width / 2
        Dim rightmargin As Integer = PD.DefaultPageSettings.PaperSize.Width
        Dim rightmarginforto As Integer = PD.DefaultPageSettings.Margins.Right
        Dim rightmarginforpr As Integer = PD.DefaultPageSettings.Margins.Right

        'font alignment
        Dim right As New StringFormat
        Dim center As New StringFormat
        Dim pr As New StringFormat

        right.Alignment = StringAlignment.Far
        center.Alignment = StringAlignment.Center
        pr.Alignment = StringAlignment.Near
        'RMS Complex , Near Canara Bank , Puduvalavu , Ponnamaravathi , Pudukkottai - Dt , Cell : 78712 93638.

        Dim line As String
        line = "--------------------------------------------------------------------------"
        Dim b As String
        b = Me.Bill_no.Text
        'range from top
        e.Graphics.DrawString("4U FASHION LOOK", f14, Brushes.Black, centermargin, 5, center)
        'e.Graphics.DrawString("RMS Complex, Near Canara Bank, Puduvalavu", f10, Brushes.Black, centermargin + 1, 25, center)
        e.Graphics.DrawString(" " + "RMS Complex, Near Canara Bank, Puduvalavu", f10, Brushes.Black, centermargin, 25, center)
        e.Graphics.DrawString("Pudukkottai - Dt,Cell : 78712 93638", f10, Brushes.Black, centermargin, 40, center)
        Dim BillAndDatehi As Integer = 70
        e.Graphics.DrawString(DateTime.Now(), f8, Brushes.Black, 120, BillAndDatehi)
        e.Graphics.DrawString("BillNo", f8, Brushes.Black, 0, BillAndDatehi)
        e.Graphics.DrawString(":", f8, Brushes.Black, 50, BillAndDatehi)
        e.Graphics.DrawString(b, f8, Brushes.Black, 70, BillAndDatehi)
        Dim CashierAndAdminhi As Integer = 80
        e.Graphics.DrawString("Cashier", f8, Brushes.Black, 0, CashierAndAdminhi)
        e.Graphics.DrawString(":", f8, Brushes.Black, 50, CashierAndAdminhi)
        e.Graphics.DrawString(currentuser.ToString, f8, Brushes.Black, 70, CashierAndAdminhi)
        'e.Graphics.DrawString(DateTime.Now(), f8, Brushes.Black, 0, 90)
        e.Graphics.DrawString(line, f8, Brushes.Black, 0, 100)
        e.Graphics.DrawString("Product Name", f10b, Brushes.Black, 0, 110)
        e.Graphics.DrawString("Quantity", f10b, Brushes.Black, 20, 135)
        e.Graphics.DrawString("Price", f10b, Brushes.Black, 85, 135)
        e.Graphics.DrawString("Total", f10b, Brushes.Black, 150, 135)
        e.Graphics.DrawString(line, f8, Brushes.Black, 0, 150)
        Dim height As Integer 'DGV Position
        BillingGridsumma.AllowUserToAddRows = False
        'If DataGridView1.CurrentCell.Value Is Nothing Then
        '    Exit Sub
        'Else
        Dim hi As Integer
        'Dim Queryes = "select ref_id As 'REF ID',pro.Product_name As 'PRODUCT NAME',cat.Category As 'CATEGORY',Brand.Brand As 'BRAND',Bill.Quantity As 'QUANTITY',Bill.Price As 'PRICE',Bill.Total 'TOTAL' from dbo.Billing As Bill inner join Products As pro on pro.Product_id = Bill.Product_id  inner join Category As cat on cat.Cat_id = pro.Cat_id inner join Brands As Brand on Brand.Brand_id = pro.Brand_id where Bill.Status = 0 And Bill.Billing_no = @BillNO"
        height = 50
        hi = 0
        For row As Integer = 0 To BillingGridsumma.RowCount - 1
            height += 20 + hi
            'Product Name
            hi = 17 + row
            e.Graphics.DrawString(BillingGridsumma.Rows(row).Cells(3).Value.ToString, f10, Brushes.Black, 0, 100 + height)
            'Quantity
            e.Graphics.DrawString(BillingGridsumma.Rows(row).Cells(6).Value.ToString, f10, Brushes.Black, 25, 100 + height + hi)
            'price
            e.Graphics.DrawString(BillingGridsumma.Rows(row).Cells(7).Value.ToString, f10, Brushes.Black, 80, 100 + height + hi)
            'Total
            e.Graphics.DrawString(BillingGridsumma.Rows(row).Cells(8).Value.ToString, f10, Brushes.Black, 150, 100 + height + hi)
            Dim convertTotAsDouble = Convert.ToDouble(BillingGridsumma.Rows(row).Cells(8).Value.ToString)
            Dim convertPercentageDouble = Convert.ToDouble(Me.DiscountIo.Text)
            Dim curDisCountAmount = DiscountPrice(convertTotAsDouble, convertPercentageDouble)
            Dim curDiscountTot = convertTotAsDouble - curDisCountAmount
            'MsgBox(curDiscountTot.ToString)
            e.Graphics.DrawString(curDiscountTot.ToString, f10, Brushes.Black, 150, 100 + height + hi)


        Next
        'End If

        Dim height2 As Integer
        height2 = 110 + height + hi + 20
        sumprice() 'call sub
        e.Graphics.DrawString(line, f8, Brushes.Black, 0, height2)
        'e.Graphics.DrawString("0", f10b, Brushes.Black, 170, 10 + height2)
        If String.IsNullOrEmpty(ReturnAmount.Text) Then
            ReturnAmount.Text = "0"
        End If
        If String.IsNullOrEmpty(Final_amount.Text) Then
            Final_amount.Text = "0"
        End If
        If String.IsNullOrEmpty(DiscountIo.Text) Then
            DiscountIo.Text = "0"
        End If
        If String.IsNullOrEmpty(RefundAmounttxtbox.Text) Then
            RefundAmounttxtbox.Text = "0"
        End If
        If Len(Me.ReturnAmount.Text) <> 0 And Len(Return_billno.Text) <> 0 Then

            Dim Query As String = "update ReturnTable set Returned=@val where Returned=0 And Status = 1 And Billing_no=@BillNo"
            Dim parameters As New List(Of SqlParameter)
            parameters.Add(New SqlParameter("@val", 1))
            parameters.Add(New SqlParameter("@BillNo", Return_billno.Text))
            QueryProcess(Query, parameters)
            'Barcode
            Dim gbarcode As New MessagingToolkit.Barcode.BarcodeEncoder
            Dim barcodeimage As Image
            barcodeimage = New Bitmap(gbarcode.Encode(MessagingToolkit.Barcode.BarcodeFormat.Code128, Me.Bill_no.Text))
            e.Graphics.DrawString("Total Quantity:" + "    " + t_qty.ToString(), f10b, Brushes.Black, 10, 10 + height2)
            e.Graphics.DrawString("Net Amount:    " + "    " + Me.grandtot.Text, f10b, Brushes.Black, 10, 10 + height2 + 15)
            e.Graphics.DrawString("Return Amount: " + " " + ReturnAmount.Text, f10b, Brushes.Black, 10, 10 + height2 + 30)
            e.Graphics.DrawString("Grand Total:   " + "      " + Final_amount.Text, f10b, Brushes.Black, 10, 10 + height2 + 45)
            e.Graphics.DrawString("Refund Amount: " + " " + RefundAmounttxtbox.Text, f10b, Brushes.Black, 10, 10 + height2 + 60)
            e.Graphics.DrawString("Discount:   " + "    " + DiscountIo.Text + "%", f10b, Brushes.Black, 10, 10 + height2 + 75)
            e.Graphics.DrawImage(barcodeimage, CInt((e.PageBounds.Width - 53) / 2), 10 + height2 + 85, 100, 35)
            e.Graphics.DrawString("~ Thanks for visting ~", f10, Brushes.Black, centermargin, 10 + height2 + 110, center)
        Else
            'Barcode
            Dim gbarcode As New MessagingToolkit.Barcode.BarcodeEncoder
            Dim barcodeimage As Image
            barcodeimage = New Bitmap(gbarcode.Encode(MessagingToolkit.Barcode.BarcodeFormat.Code128, Me.Bill_no.Text))
            e.Graphics.DrawString("Total Quantity:" + " " + t_qty.ToString(), f10b, Brushes.Black, 10, 10 + height2)
            e.Graphics.DrawString("Net Amount:    " + " " + Me.grandtot.Text, f10b, Brushes.Black, 10, 10 + height2 + 20)
            e.Graphics.DrawString("Grand Total:   " + " " + Final_amount.Text, f10b, Brushes.Black, 10, 10 + height2 + 35)
            e.Graphics.DrawString("Discount: " + " " + DiscountIo.Text + "%", f10b, Brushes.Black, 10, 10 + height2 + 50)
            e.Graphics.DrawImage(barcodeimage, CInt((e.PageBounds.Width - 80) / 2), 10 + height2 + 75, 100, 35)
            e.Graphics.DrawString("~ Thanks for visting ~", f10, Brushes.Black, centermargin, 10 + height2 + 110, center)
        End If
    End Sub
    Dim t_price As Integer
    Dim t_qty As Integer
    Sub sumprice()
        Dim countprice As Integer = 0
        For rowitem As Integer = 0 To BillingGridsumma.RowCount - 1
            countprice = countprice + Val(BillingGridsumma.Rows(rowitem).Cells(7).Value * BillingGridsumma.Rows(rowitem).Cells(6).Value)
        Next
        t_price = countprice

        Dim countqty As Integer = 0
        For rowitem As Integer = 0 To BillingGridsumma.RowCount - 1
            countqty = countqty + BillingGridsumma.Rows(rowitem).Cells(6).Value
        Next
        t_qty = countqty
    End Sub

    Public Sub LoadAutoComplete()
        Try
            MobileNo.AutoCompleteCustomSource = ReturnUserData()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub






    Private Sub BarcodeCodetxt_KeyDown(sender As Object, e As KeyEventArgs) Handles BarcodeCodetxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            GettheProduct(BarcodeCodetxt.Text)
            Me.Quantity.Focus()
        End If
    End Sub

    Private Sub Quantity_TextChanged(sender As Object, e As EventArgs) Handles Quantity.TextChanged
        IncrementTheTotal(Me.Quantity.Text, Me.Price.Text)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        AddProduct(Me.BarcodeCodetxt.Text, Me.Bill_no.Text)
        InitialLoad()
        Me.Quantity.Text = 1
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        InitialLoad()
        Me.Quantity.Text = 1
    End Sub

    Private Sub Quantity_KeyDown(sender As Object, e As KeyEventArgs) Handles Quantity.KeyDown
        If e.KeyCode = Keys.Enter Then
            Button2.PerformClick()
            Me.BarcodeCodetxt.Focus()
        End If
    End Sub

    Private Sub Billbtn_Click(sender As Object, e As EventArgs) Handles Billbtn.Click
        FinalizeBill()
    End Sub




    Private Sub AddUserbtn_Click(sender As Object, e As EventArgs) Handles AddUserbtn.Click
        Dim frm2 = AddUser
        frm2.Show()
    End Sub



    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.Alt And e.KeyCode
            Case Keys.F3
                Billbtn.PerformClick()
        End Select
        Select Case e.Control And e.KeyCode
            Case Keys.F3
                Return_billno.Focus()
        End Select

        Select Case e.Alt And e.KeyCode
            Case Keys.F1
                BarcodeCodetxt.Focus()

        End Select
        Select Case e.Control And e.KeyCode
            Case Keys.F1
                Quantity.Focus()

        End Select
        Select Case e.Alt And e.KeyCode

            Case Keys.F2
                MobileNo.Focus()
        End Select
        Select Case e.Control And e.KeyCode

            Case Keys.F2
                Amount.Focus()
        End Select
        Select Case e.Control And e.KeyCode

            Case Keys.L
                LoadAutoComplete()
        End Select
    End Sub

    Private Sub BillingGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BillingGridsumma.CellContentClick
        Try
            If BillingGridsumma.Columns(e.ColumnIndex).Name = "del" Then ' Ensure a valid cell is clicked
                Dim RefId As String = BillingGridsumma.Rows(e.RowIndex).Cells(2).Value.ToString()
                Dim DeleteQuery As String = "DELETE FROM Billing WHERE ref_id=@RefId;"
                Dim parameters As New List(Of SqlParameter)
                parameters.Add(New SqlParameter("@RefId", Convert.ToInt32(RefId)))
                If DeleteProduct(DeleteQuery, parameters) = 1 Then
                    LoadGrid(Me.Bill_no.Text)
                    CalculateGrandTotal(Me.Bill_no.Text)
                    MsgBox("Deleted The Product!")
                    DiscountIotextChangedFunction()
                    AmountValue()

                End If

            ElseIf BillingGridsumma.Columns(e.ColumnIndex).Name = "val" Then
                Dim RefId As String = BillingGridsumma.Rows(e.RowIndex).Cells(2).Value.ToString()
                Dim Quantity As String = BillingGridsumma.Rows(e.RowIndex).Cells(6).Value.ToString()
                Dim Price As String = BillingGridsumma.Rows(e.RowIndex).Cells(7).Value.ToString()
                BillingGridsumma.Rows(e.RowIndex).Selected = True
                Dim dr As DataGridViewRow = BillingGridsumma.SelectedRows(0)
                UpdateProduct(RefId, Quantity, Price)
                DiscountIotextChangedFunction()
                AmountValue()
            End If
        Catch ex As Exception
            'MsgBox(ex.ToString)

        End Try

    End Sub


    Private Sub MobileNo_KeyDown(sender As Object, e As KeyEventArgs) Handles MobileNo.KeyDown
        If e.KeyCode = Keys.Enter Then
            Billbtn.PerformClick()
        End If
    End Sub

    Private Sub BILLING_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
        Dim Query As String = "Delete Billing where Billing_no=@BillNo"
        Me.DeleteBillWhenFormClose(Me.Bill_no.Text, Query)
    End Sub

    Public Sub DeleteBillWhenFormClose(BillNo, Query)
        Dim parameters As New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@BillNO", BillNo))
        QueryProcess(Query, parameters)

    End Sub
    Private Sub Return_billno_KeyDown(sender As Object, e As KeyEventArgs) Handles Return_billno.KeyDown
        If e.KeyCode = Keys.Enter And Len(Return_billno.Text) <> 0 Then
            If String.IsNullOrEmpty(Me.Return_billno.Text) Then
                MsgBox("please Provide Appropriate Return BillNo")
                Me.ReturnAmount.Clear()
                Me.Return_billno.Clear()
                InitialLoad()
                Me.Quantity.Text = 1
            End If
            If Convert.ToInt32(GetTheReturnAmount(Me.Return_billno.Text)) > 0 Then
                Me.ReturnAmount.Text = GetTheReturnAmount(Me.Return_billno.Text).ToString
                CalculateGrandTotal(Me.Bill_no.Text)
            Else
                MsgBox("please Provide Appropriate Return BillNo")
                Me.ReturnAmount.Clear()
                Me.Return_billno.Clear()
                InitialLoad()
                Me.Quantity.Text = 1
            End If
        End If
    End Sub



    Private Sub Amount_TextChanged(sender As Object, e As EventArgs) Handles Amount.TextChanged
        AmountValue()
    End Sub


    Public Sub AmountValue()
        Try
            CalculateGrandTotal(Me.Bill_no.Text)

            If Not String.IsNullOrEmpty(Final_amount.Text) Then
                'MsgBox("Enter Amount Textchanged")
                Dim bal As Double = Convert.ToDouble(Me.Amount.Text) - Convert.ToDouble(Me.Final_amount.Text)
                If bal < 0 Then
                    bal = 0
                End If
                Me.Balance.Text = bal.ToString("0.00")
            End If
        Catch ex As Exception
            Me.Balance.Text = "0"
        End Try
    End Sub
    Private Sub MobileNo_GotFocus(sender As Object, e As EventArgs) Handles MobileNo.GotFocus
        LoadAutoComplete()
    End Sub
    Public Function ReturnPercentagevalue(discountpercentage As Double)
        Dim reductionFactor As Double = (100 - discountpercentage) / 100
        Console.WriteLine("Reduction Factor: " & reductionFactor.ToString("F2"))
        MsgBox(reductionFactor.ToString("F2"))
    End Function
    Public Function DiscountPrice(grandtot As Double, discountpercent As Double) As Int32
        If discountpercent < 100 And discountpercent <> 0 Then
            Dim grandTotal As Double = grandtot
            Dim discountPercentage As Double = discountpercent

            'changes required
            Dim discountAmount As Double = (discountPercentage / 100) * grandTotal
            Dim discountedTotal As Double = grandTotal - discountAmount
            'Me.grandtot.Text = discountedTotal
            Return Convert.ToInt32(discountAmount).ToString(0.00)
        Else
            CalculateGrandTotal(Me.Bill_no.Text)
            Return Convert.ToInt32(grandtot).ToString(0.00)
        End If

    End Function
    Private Sub DiscountIo_TextChanged(sender As Object, e As EventArgs) Handles DiscountIo.TextChanged
        DiscountIotextChangedFunction()
    End Sub

    Public Sub DiscountIotextChangedFunction()
        CalculateGrandTotal(Me.Bill_no.Text)
        Try
            Me.DiscountOut.Text = DiscountPrice(Convert.ToDouble(Me.grandtot.Text), Convert.ToDouble(Me.DiscountIo.Text))
            CalculateGrandTotal(Me.Bill_no.Text)
            AmountValue()

            If DiscountOut.Text < 0 Then
                Me.DiscountOut.Text = "0"
                CalculateGrandTotal(Me.Bill_no.Text)
                AmountValue()

            End If
        Catch ex As Exception
            Me.DiscountOut.Text = "0"
            CalculateGrandTotal(Me.Bill_no.Text)
            AmountValue()

        End Try
    End Sub

    Public Sub Final_amount_val(ReturnAmount As String, DiscountAmount As String, GrandTotal As String)
        If String.IsNullOrEmpty(ReturnAmount) Then
            ReturnAmount = "0"
        End If
        If String.IsNullOrEmpty(DiscountAmount) Then
            DiscountAmount = "0"
        End If
        If String.IsNullOrEmpty(GrandTotal) Then
            GrandTotal = "0"
        End If

        Dim FinalVal As Double = Convert.ToDouble(GrandTotal) - (Convert.ToDouble(ReturnAmount) + Convert.ToDouble(DiscountAmount))
        Dim Tempval As Double = Convert.ToDouble(GrandTotal) - Convert.ToDouble(DiscountAmount)
        Dim RefundAmountval As Double
        'MsgBox($"TemoVal:{Tempval}")
        If Tempval > 0 Then
            RefundAmountval = Tempval - Convert.ToDouble(ReturnAmount)
            'MsgBox($"RefundAmountval:{RefundAmountval}")
            If RefundAmountval < 0 Then
                RefundAmountval = Math.Abs(RefundAmountval)
            Else
                RefundAmountval = 0
            End If
        Else
            RefundAmountval = Convert.ToDouble(ReturnAmount)
        End If
        Me.RefundAmounttxtbox.Text = RefundAmountval.ToString("0.00")
        'MessageBox.Show($"GrandTotal: {GrandTotal}{Environment.NewLine}ReturnAmount: {ReturnAmount}{Environment.NewLine}DiscountAmount: {DiscountAmount} Finalval:{FinalVal}{Environment.NewLine}", "Product Details", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Me.Final_amount.Text = FinalVal.ToString("0.00")
        If FinalVal < 1 Then
            Me.Final_amount.Text = "0"
        End If
    End Sub

    Private Sub BarcodeCodetxt_TextChanged(sender As Object, e As EventArgs) Handles BarcodeCodetxt.TextChanged

    End Sub

    Private Sub Return_billno_TextChanged(sender As Object, e As EventArgs) Handles Return_billno.TextChanged

    End Sub

    Private Sub Final_amount_TextChanged(sender As Object, e As EventArgs) Handles Final_amount.TextChanged

    End Sub

    Private Sub MobileNo_TextChanged(sender As Object, e As EventArgs) Handles MobileNo.TextChanged

    End Sub
End Class