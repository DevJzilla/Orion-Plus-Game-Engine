﻿Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.Reflection
Imports System.Windows.Forms

Friend Class frmLauncher
    Private _newsCount As UInteger
    Private _entryApp As String
    Private _entryCode As String
    
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Environment.Exit(0)
    End Sub
    
    Private Sub btnMinimize_Click(sender As Object, e As EventArgs)  Handles btnMinimize.Click
        WindowState = FormWindowState.Minimized
    End Sub

    Private Sub picPlay_Click(sender As Object, e As EventArgs) 
        Process.Start(_entryApp, _entryCode)
        Environment.Exit(0)

        dim dll = Assembly.LoadFile(Environment.CurrentDirectory & "\Game.dll")
        'dim entry = dll..GetType("Entry")
        'entry.Invoke
        'for each type in entry
        'dim c = Activator.CreateInstance(type)
        'type.InvokeMember("Output", BindingFlags.InvokeMethod, nothing, c, new object[] {"Hello"})
        'next
    End Sub
    
    Private Sub tmrNews_Tick(sender As Object, e As EventArgs) Handles tmrNews.Tick
        lblNews.Text = "Fetching News"

        _newsCount += 1

        Select Case _newsCount
            Case 1 : lblNews.Text += "."
            Case 2 : lblNews.Text += ".."
            Case 3 : lblNews.Text += "..."
            Case 4 : _newsCount = 0
        End Select
    End Sub

#Region " Async Functions "

    Friend Sub SetNews(msg As String)
        tmrNews.Enabled = False

        lblNews.TextAlign = ContentAlignment.TopLeft
        lblNews.Text = msg
    End Sub
    
    Friend Sub UpdateProgress(progress As UInteger)
        if Not lblStatus.Visible then lblStatus.Show()
        if Not barDownload.Visible then barDownload.Show()

        barDownload.Value = progress
    End Sub

    Friend Sub EnableGame(app As String, code As String)
        _entryApp = app
        _entryCode = code

        lblStatus.Hide()
        barDownload.Hide()
        btnPlay.Enabled = True
        btnPlay.BackColor = Color.FromArgb(255, 30, 70, 130)
    End Sub

#End Region

End Class