﻿

Imports System.Security.Cryptography

Public Class admin_panel
    Private Sub AddProductsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddProductsToolStripMenuItem.Click
        Dim frm = Add_Product
        frm.Show()
        frm.MdiParent = Me
    End Sub

    Private Sub UpdateProductsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateProductsToolStripMenuItem.Click
        Dim frm = update_product
        frm.Show()
        frm.MdiParent = Me
    End Sub

    Private Sub ExitToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem2.Click
        Dim frm = Barcode
        frm.Show()
        frm.MdiParent = Me
    End Sub

    Private Sub ExitToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem1.Click
        Dim frm = Backup
        frm.Show()
        frm.MdiParent = Me

    End Sub



    Private Sub SalesReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SalesReportToolStripMenuItem.Click
        Dim frm = salesreport
        frm.Show()
        frm.MdiParent = Me

    End Sub

    Private Sub StocksReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StocksReportToolStripMenuItem.Click
        Dim frm = stock_report
        frm.Show()
        frm.MdiParent = Me
    End Sub

    Private Sub BillingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BillingToolStripMenuItem.Click
        Dim frm = New BILLING
        frm.Show()
        frm.MdiParent = Me
    End Sub

    Private Sub admin_panel_Load(sender As Object, e As EventArgs) Handles MyBase.Load


    End Sub

    Private Sub DashboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DashboardToolStripMenuItem.Click
        Dim frm = New Dashboard
        frm.Show()
        frm.MdiParent = Me
    End Sub
End Class