﻿Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient

Module CrudFile
    Public Sub InsertDataas(Sqlquery As String, Parameters As List(Of SqlParameter))
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
        Catch ex As Exception
            ' Log the exception or handle it appropriately
            MsgBox("Something Went Wrong!" & ex.Message)
            Debug.WriteLine("An error occurred: " & ex.Message)

        End Try
    End Sub

    Public Function QueryProcess(Sqlquery As String, Parameters As List(Of SqlParameter)) As Integer
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

    Public Function ReturnUserData() As AutoCompleteStringCollection
        Dim con As New SqlConnection(connectionString)
        Dim cmd As SqlCommand
        Dim da As SqlDataAdapter
        Dim ds As DataSet
        Dim dt As DataTable
        con.Open()
        Dim query As String = "SELECT DISTINCT MobileNo FROM Customer"
        cmd = New SqlCommand(query, con)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet()
        da.Fill(ds, "MobileNo")
        dt = ds.Tables("MobileNo")
        Dim col As New AutoCompleteStringCollection
        For Each row As DataRow In dt.Rows
            col.Add(row("MobileNo").ToString())
        Next
        Return col
    End Function
End Module
