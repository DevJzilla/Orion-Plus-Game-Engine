﻿Imports System
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports ASFW

Friend Class FrmMenu
    Inherits Form

#Region "Form Functions"

    ''' <summary>
    ''' clean up and close the game.
    ''' </summary>
    Private Sub FrmMenu_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Terminate()
    End Sub

    ''' <summary>
    ''' On load, get GUI ready.
    ''' </summary>
    Private Sub Frmmenu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Configuration.LoadLanguage("English")

        LoadMenuGraphics()

        pnlLoad.Width = 730
        pnlLoad.Height = 550

        Width = 730
        pnlLogin.Top = pnlMainMenu.Top
        pnlLogin.Left = pnlMainMenu.Left

        pnlNewChar.Top = pnlMainMenu.Top
        pnlNewChar.Left = pnlMainMenu.Left

        pnlRegister.Top = pnlMainMenu.Top
        pnlRegister.Left = pnlMainMenu.Left

        pnlCredits.Top = pnlMainMenu.Top
        pnlCredits.Left = pnlMainMenu.Left

        pnlIPConfig.Top = pnlMainMenu.Top
        pnlIPConfig.Left = pnlMainMenu.Left

        pnlCharSelect.Top = pnlMainMenu.Top
        pnlCharSelect.Left = pnlMainMenu.Left

        If Started = False Then Call Startup()

        Network.Connect(Configuration.Settings.Ip, Configuration.Settings.Port)

    End Sub

    ''' <summary>
    ''' Draw Char select when its needed.
    ''' </summary>
    Private Sub PnlCharSelect_VisibleChanged(sender As Object, e As EventArgs) Handles pnlCharSelect.VisibleChanged
        DrawCharacterSelect()
    End Sub

    ''' <summary>
    ''' Shows the IP config.
    ''' </summary>
    Private Sub LblServerStatus_Click(sender As Object, e As EventArgs) Handles lblServerStatus.Click
        PnlCreditsVisible = False
        PnlLoginVisible = False
        PnlRegisterVisible = False
        PnlCharCreateVisible = False

        txtIP.Text = Configuration.Settings.Ip
        txtPort.Text = Configuration.Settings.Port

        pnlIPConfig.Visible = True
    End Sub

#End Region

#Region "Draw Functions"

    ''' <summary>
    ''' Preload the images in the menu.
    ''' </summary>
    Friend Sub LoadMenuGraphics()

        'main menu
        If File.Exists(Environment.CurrentDirectory & GfxGuiPath & "Menu\menu" & GfxExt) Then
            BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\menu" & GfxExt)
        End If

        'main menu buttons
        If File.Exists(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt) Then
            btnCredits.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
            btnExit.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
            btnLogin.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
            btnPlay.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
            btnRegister.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
            btnNewChar.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
            btnUseChar.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
            btnDelChar.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
            btnCreateAccount.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
            btnSaveIP.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
        End If

        'main menu panels
        If File.Exists(Environment.CurrentDirectory & GfxGuiPath & "Menu\panel" & GfxExt) Then
            pnlMainMenu.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\panel" & GfxExt)
            pnlLogin.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\panel" & GfxExt)
            pnlNewChar.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\panel" & GfxExt)
            pnlCharSelect.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\panel" & GfxExt)
            pnlRegister.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\panel" & GfxExt)
            pnlCredits.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\panel" & GfxExt)
            pnlIPConfig.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\panel" & GfxExt)
        End If

        'logo
        If File.Exists(Environment.CurrentDirectory & GfxGuiPath & "Menu\logo" & GfxExt) Then
            picLogo.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\logo" & GfxExt)
        End If

        'set text for controls from language file

        'main
        lblStatusHeader.Text = Configuration.Language.MainMenu.serverstatus
        btnPlay.Text = Configuration.Language.MainMenu.buttonplay
        btnRegister.Text = Configuration.Language.MainMenu.buttonregister
        btnCredits.Text = Configuration.Language.MainMenu.buttoncredits
        btnExit.Text = Configuration.Language.MainMenu.buttonexit

        'logon panel
        lblLogin.Text = Configuration.Language.MainMenu.login
        lblLoginName.Text = Configuration.Language.MainMenu.loginname
        lblLoginPass.Text = Configuration.Language.MainMenu.loginpass
        chkSavePass.Text = Configuration.Language.MainMenu.loginchkbox
        btnLogin.Text = Configuration.Language.MainMenu.loginbutton

        'new char panel
        lblNewChar.Text = Configuration.Language.MainMenu.newchar
        lblNewCharName.Text = Configuration.Language.MainMenu.newcharname
        lblNewCharClass.Text = Configuration.Language.MainMenu.newcharclass
        lblNewCharGender.Text = Configuration.Language.MainMenu.newchargender
        rdoMale.Text = Configuration.Language.MainMenu.newcharmale
        rdoFemale.Text = Configuration.Language.MainMenu.newcharfemale
        lblNewCharSprite.Text = Configuration.Language.MainMenu.newcharsprite
        btnCreateCharacter.Text = Configuration.Language.MainMenu.newcharbutton

        'char select
        lblCharSelect.Text = Configuration.Language.MainMenu.selchar
        btnNewChar.Text = Configuration.Language.MainMenu.selcharnew
        btnUseChar.Text = Configuration.Language.MainMenu.selcharuse
        btnDelChar.Text = Configuration.Language.MainMenu.selchardel

        'new account
        lblNewAccount.Text = Configuration.Language.MainMenu.newacc
        lblNewAccName.Text = Configuration.Language.MainMenu.newaccname
        lblNewAccPass.Text = Configuration.Language.MainMenu.newaccpass
        lblNewAccPass2.Text = Configuration.Language.MainMenu.newaccpass2

        'credits
        lblCreditsTop.Text = Configuration.Language.MainMenu.credits

        'ip config
        lblIpConfig.Text = Configuration.Language.MainMenu.ipconfig
        lblIpAdress.Text = Configuration.Language.MainMenu.ipconfigadres
        lblPort.Text = Configuration.Language.MainMenu.ipconfigport
    End Sub

    ''' <summary>
    ''' Draw the Character for new char creation.
    ''' </summary>
    Sub DrawCharacter()
        If pnlNewChar.Visible = True Then
            Dim g As System.Drawing.Graphics = pnlNewChar.CreateGraphics
            Dim filename As String
            Dim srcRect As Rectangle
            Dim destRect As Rectangle
            Dim charwidth As Integer
            Dim charheight As Integer

            If NewCharClass = 0 Then NewCharClass = 1
            If NewCharSprite = 0 Then NewCharSprite = 1

            If rdoMale.Checked = True Then
                filename = Environment.CurrentDirectory & GfxPath & "characters\" & Classes(NewCharClass).MaleSprite(NewCharSprite) & GfxExt
            Else
                filename = Environment.CurrentDirectory & GfxPath & "characters\" & Classes(NewCharClass).FemaleSprite(NewCharSprite) & GfxExt
            End If

            Dim charsprite As Bitmap = New Bitmap(filename)

            charwidth = charsprite.Width / 4
            charheight = charsprite.Height / 4

            srcRect = New Rectangle(0, 0, charwidth, charheight)
            destRect = New Rectangle(placeholderforsprite.Left, placeholderforsprite.Top, charwidth, charheight)

            charsprite.MakeTransparent(charsprite.GetPixel(0, 0))

            If charwidth > 32 Then
                Lblnextcharleft = (100 - (64 - charwidth))
            Else
                Lblnextcharleft = 100
            End If
            pnlNewChar.Refresh()
            g.DrawImage(charsprite, destRect, srcRect, GraphicsUnit.Pixel)
            g.Dispose()
        End If
    End Sub

    ''' <summary>
    ''' Draw the character for the char select screen.
    ''' </summary>
    Sub DrawCharacterSelect()
        Dim g As System.Drawing.Graphics
        Dim srcRect As Rectangle
        Dim destRect As Rectangle

        If pnlCharSelect.Visible = True Then
            Dim filename As String
            Dim charwidth As Integer, charheight As Integer

            'first
            If CharSelection(1).Sprite > 0 Then
                g = picChar1.CreateGraphics

                filename = Environment.CurrentDirectory & GfxPath & "characters\" & CharSelection(1).Sprite & GfxExt

                Dim charsprite As Bitmap = New Bitmap(filename)

                charwidth = charsprite.Width / 4
                charheight = charsprite.Height / 4

                srcRect = New Rectangle(0, 0, charwidth, charheight)
                destRect = New Rectangle(0, 0, charwidth, charheight)

                charsprite.MakeTransparent(charsprite.GetPixel(0, 0))

                picChar1.Refresh()
                g.DrawImage(charsprite, destRect, srcRect, GraphicsUnit.Pixel)

                If SelectedChar = 1 Then
                    g.DrawRectangle(Pens.Red, New Rectangle(0, 0, charwidth - 1, charheight))
                End If

                g.Dispose()
            Else
                picChar1.BorderStyle = BorderStyle.FixedSingle
                picChar1.Refresh()
            End If

            'second
            If CharSelection(2).Sprite > 0 Then
                g = picChar2.CreateGraphics

                filename = Environment.CurrentDirectory & GfxPath & "characters\" & CharSelection(2).Sprite & GfxExt

                Dim charsprite As Bitmap = New Bitmap(filename)

                charwidth = charsprite.Width / 4
                charheight = charsprite.Height / 4

                srcRect = New Rectangle(0, 0, charwidth, charheight)
                destRect = New Rectangle(0, 0, charwidth, charheight)

                charsprite.MakeTransparent(charsprite.GetPixel(0, 0))

                picChar2.Refresh()
                g.DrawImage(charsprite, destRect, srcRect, GraphicsUnit.Pixel)

                If SelectedChar = 2 Then
                    g.DrawRectangle(Pens.Red, New Rectangle(0, 0, charwidth - 1, charheight))
                End If

                g.Dispose()
            Else
                picChar2.BorderStyle = BorderStyle.FixedSingle
                picChar2.Refresh()
            End If

            'third
            If CharSelection(3).Sprite > 0 Then
                g = picChar3.CreateGraphics

                filename = Environment.CurrentDirectory & GfxPath & "characters\" & CharSelection(3).Sprite & GfxExt

                Dim charsprite As Bitmap = New Bitmap(filename)

                charwidth = charsprite.Width / 4
                charheight = charsprite.Height / 4

                srcRect = New Rectangle(0, 0, charwidth, charheight)
                destRect = New Rectangle(0, 0, charwidth, charheight)

                charsprite.MakeTransparent(charsprite.GetPixel(0, 0))

                picChar3.Refresh()
                g.DrawImage(charsprite, destRect, srcRect, GraphicsUnit.Pixel)

                If SelectedChar = 3 Then
                    g.DrawRectangle(Pens.Red, New Rectangle(0, 0, charwidth - 1, charheight))
                End If

                g.Dispose()
            Else
                picChar3.BorderStyle = BorderStyle.FixedSingle
                picChar3.Refresh()
            End If

        End If
    End Sub

    ''' <summary>
    ''' Stop the NewChar panel from repainting itself.
    ''' </summary>
    Private Sub PnlNewChar_Paint(sender As Object, e As PaintEventArgs) Handles pnlNewChar.Paint
        'nada here
    End Sub

#End Region

#Region "Credits"

    ''' <summary>
    ''' This timer handles the scrolling credits.
    ''' </summary>
    Private Sub TmrCredits_Tick(sender As Object, e As EventArgs) Handles tmrCredits.Tick
        Dim credits As String
        Dim filepath As String
        filepath = Environment.CurrentDirectory & "\Data\credits.txt"
        lblScrollingCredits.Top = 177
        If PnlCreditsVisible = True Then
            tmrCredits.Enabled = False
            credits = GetFileContents(filepath)
            lblScrollingCredits.Text = "" & Environment.NewLine & credits
            Do Until PnlCreditsVisible = False
                lblScrollingCredits.Top = lblScrollingCredits.Top - 1
                If lblScrollingCredits.Bottom <= lblCreditsTop.Bottom Then
                    lblScrollingCredits.Top = 177
                End If
                Application.DoEvents()
                Threading.Thread.Sleep(100)
            Loop
        End If
    End Sub

#End Region

#Region "Login"

    ''' <summary>
    ''' Handles press enter on login name txtbox.
    ''' </summary>
    Private Sub TxtLogin_KeyDown(sender As Object, e As KeyEventArgs) Handles txtLogin.KeyDown
        If e.KeyCode = Keys.Enter Then
            BtnLogin_Click(Me, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Handles press enter on login password txtbox.
    ''' </summary>
    Private Sub TxtPassword_KeyDown(sender As Object, e As KeyEventArgs) Handles txtPassword.KeyDown
        If e.KeyCode = Keys.Enter Then
            BtnLogin_Click(Me, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Handle the SavePas checkbox.
    ''' </summary>
    Private Sub ChkSavePass_CheckedChanged(sender As Object, e As EventArgs) Handles chkSavePass.CheckedChanged
        ChkSavePassChecked = chkSavePass.Checked
    End Sub

#End Region

#Region "Char Creation"

    ''' <summary>
    ''' Changes selected class.
    ''' </summary>
    Private Sub CmbClass_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbClass.SelectedIndexChanged
        NewCharClass = cmbClass.SelectedIndex + 1
        txtDescription.Text = Classes(NewCharClass).Desc
        DrawCharacter()
    End Sub

    ''' <summary>
    ''' Switches to male gender.
    ''' </summary>
    Private Sub RdoMale_CheckedChanged(sender As Object, e As EventArgs) Handles rdoMale.CheckedChanged
        DrawCharacter()
    End Sub

    ''' <summary>
    ''' Switches to female gender.
    ''' </summary>
    Private Sub RdoFemale_CheckedChanged(sender As Object, e As EventArgs) Handles rdoFemale.CheckedChanged
        DrawCharacter()
    End Sub

    ''' <summary>
    ''' Switches sprite for selected class to next one, if any.
    ''' </summary>
    Private Sub LblNextChar_Click(sender As Object, e As EventArgs) Handles lblNextChar.Click
        NewCharSprite = NewCharSprite + 1
        If rdoMale.Checked = True Then
            If NewCharSprite > Classes(NewCharClass).MaleSprite.Length - 1 Then NewCharSprite = 1
        ElseIf rdoFemale.Checked = True Then
            If NewCharSprite > Classes(NewCharClass).FemaleSprite.Length - 1 Then NewCharSprite = 1
        End If
        DrawCharacter()
    End Sub

    ''' <summary>
    ''' Switches sprite for selected class to previous one, if any.
    ''' </summary>
    Private Sub LblPrevChar_Click(sender As Object, e As EventArgs) Handles lblPrevChar.Click
        NewCharSprite = NewCharSprite - 1
        If rdoMale.Checked = True Then
            If NewCharSprite = 0 Then NewCharSprite = Classes(NewCharClass).MaleSprite.Length - 1
        ElseIf rdoFemale.Checked = True Then
            If NewCharSprite = 0 Then NewCharSprite = Classes(NewCharClass).FemaleSprite.Length - 1
        End If
        DrawCharacter()
    End Sub

    ''' <summary>
    ''' Initial drawing of new char.
    ''' </summary>
    Private Sub PnlNewChar_VisibleChanged(sender As Object, e As EventArgs) Handles pnlNewChar.VisibleChanged
        DrawCharacter()
    End Sub

#End Region

#Region "Buttons"

    ''' <summary>
    ''' Handle Play button press.
    ''' </summary>
    Private Sub BtnPlay_Click(sender As Object, e As EventArgs) Handles btnPlay.Click
        If Network.IsConnected() = True Then
            Audio.PlaySound("Click.ogg")
            PnlRegisterVisible = False
            PnlLoginVisible = True
            PnlCharCreateVisible = False
            PnlCreditsVisible = False
            pnlIPConfig.Visible = False
            txtLogin.Focus()
            If Configuration.Settings.SavePass = True Then
                txtLogin.Text = Configuration.Settings.Username
                txtPassword.Text = Configuration.Settings.Password
                chkSavePass.Checked = True
            End If
        End If
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnPlay_MouseEnter(sender As Object, e As EventArgs) Handles btnPlay.MouseEnter
        btnPlay.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnPlay_MouseLeave(sender As Object, e As EventArgs) Handles btnPlay.MouseLeave
        btnPlay.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handle Register button press.
    ''' </summary>
    Private Sub BtnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        If Network.IsConnected() = True Then
            Audio.PlaySound("Click.ogg")
            PnlRegisterVisible = True
            PnlLoginVisible = False
            PnlCharCreateVisible = False
            PnlCreditsVisible = False
            pnlIPConfig.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnRegister_MouseEnter(sender As Object, e As EventArgs) Handles btnRegister.MouseEnter
        btnRegister.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnRegister_MouseLeave(sender As Object, e As EventArgs) Handles btnRegister.MouseLeave
        btnRegister.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handle Credits button press.
    ''' </summary>
    Private Sub BtnCredits_Click(sender As Object, e As EventArgs) Handles btnCredits.Click
        Audio.PlaySound("Click.ogg")
        If PnlCreditsVisible = False Then
            tmrCredits.Enabled = True
        End If
        PnlCreditsVisible = True
        PnlLoginVisible = False
        PnlRegisterVisible = False
        PnlCharCreateVisible = False
        pnlIPConfig.Visible = False
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnCredits_MouseEnter(sender As Object, e As EventArgs) Handles btnCredits.MouseEnter
        btnCredits.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnCredits_MouseLeave(sender As Object, e As EventArgs) Handles btnCredits.MouseLeave
        btnCredits.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles Exit button press.
    ''' </summary>
    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Audio.PlaySound("Click.ogg")
        Terminate()
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnExit_MouseEnter(sender As Object, e As EventArgs) Handles btnExit.MouseEnter
        btnExit.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnExit_MouseLeave(sender As Object, e As EventArgs) Handles btnExit.MouseLeave
        btnExit.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles Login button press.
    ''' </summary>
    Private Sub BtnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        If IsLoginLegal(txtLogin.Text, txtPassword.Text) Then
            MenuState(MenuStateLogin)
        End If
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnLogin_MouseEnter(sender As Object, e As EventArgs) Handles btnLogin.MouseEnter
        btnLogin.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnLogin_MouseLeave(sender As Object, e As EventArgs) Handles btnLogin.MouseLeave
        btnLogin.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles CreateAccount button press.
    ''' </summary>
    Private Sub BtnCreateAccount_Click(sender As Object, e As EventArgs) Handles btnCreateAccount.Click
        Dim name As String
        Dim password As String
        Dim passwordAgain As String
        name = txtRuser.Text.Trim
        password = txtRPass.Text.Trim
        passwordAgain = txtRPass2.Text.Trim

        If IsLoginLegal(name, password) Then
            If password <> passwordAgain Then
                MessageBox.Show("Passwords don't match.")
                Exit Sub
            End If

            If Not IsStringLegal(name) Then Exit Sub

            MenuState(MenuStateNewaccount)
        End If
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnCreateAccount_MouseEnter(sender As Object, e As EventArgs) Handles btnCreateAccount.MouseEnter
        btnCreateAccount.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnCreateAccount_MouseLeave(sender As Object, e As EventArgs) Handles btnCreateAccount.MouseLeave
        btnCreateAccount.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles CreateCharacter button press.
    ''' </summary>
    Private Sub BtnCreateCharacter_Click(sender As Object, e As EventArgs) Handles btnCreateCharacter.Click
        MenuState(MenuStateAddchar)
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnCreateCharacter_MouseEnter(sender As Object, e As EventArgs) Handles btnCreateCharacter.MouseEnter
        btnCreateCharacter.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnCreateCharacter_MouseLeave(sender As Object, e As EventArgs) Handles btnCreateCharacter.MouseLeave
        btnCreateCharacter.BackgroundImage = Image.FromFile(Environment.CurrentDirectory & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles SaveIP button press.
    ''' </summary>
    Private Sub BtnSaveIP_Click(sender As Object, e As EventArgs) Handles btnSaveIP.Click
        Configuration.Settings.Ip = txtIP.Text
        Configuration.Settings.Port = txtPort.Text

        pnlIPConfig.Visible = False
        Configuration.SaveSettings()
    End Sub

    ''' <summary>
    ''' Handles selecting character 1.
    ''' </summary>
    Private Sub PicChar1_Click(sender As Object, e As EventArgs) Handles picChar1.Click
        SelectedChar = 1
        DrawCharacterSelect()
    End Sub

    ''' <summary>
    ''' Handles selecting character 2.
    ''' </summary>
    Private Sub PicChar2_Click(sender As Object, e As EventArgs) Handles picChar2.Click
        SelectedChar = 2
        DrawCharacterSelect()
    End Sub

    ''' <summary>
    ''' Handles selecting character 3.
    ''' </summary>
    Private Sub PicChar3_Click(sender As Object, e As EventArgs) Handles picChar3.Click
        SelectedChar = 3
        DrawCharacterSelect()
    End Sub

    ''' <summary>
    ''' Handles NewChar button press.
    ''' </summary>
    Private Sub BtnNewChar_Click(sender As Object, e As EventArgs) Handles btnNewChar.Click
        Dim i As Integer, newSelectedChar As Byte

        newSelectedChar = 0

        For i = 1 To MaxChars
            If CharSelection(i).Name = "" Then
                newSelectedChar = i
                Exit For
            End If
        Next

        If newSelectedChar > 0 Then
            SelectedChar = newSelectedChar
        End If

        PnlCharCreateVisible = True
        PnlCharSelectVisible = False
        DrawChar = True
    End Sub

    ''' <summary>
    ''' Handles UseChar button press.
    ''' </summary>
    Private Sub BtnUseChar_Click(sender As Object, e As EventArgs) Handles btnUseChar.Click
        Pnlloadvisible = True
        Frmmenuvisible = False

        Dim buffer As ByteStream
        buffer = New ByteStream(8)
        buffer.WriteInt32(ClientPacket.CUseChar)
        buffer.WriteInt32(SelectedChar)
        Network.SendData(buffer.ToPacket)

        buffer.Dispose()
    End Sub

    ''' <summary>
    ''' Handles DelChar button press.
    ''' </summary>
    Private Sub BtnDelChar_Click(sender As Object, e As EventArgs) Handles btnDelChar.Click
        Dim result1 As DialogResult = MessageBox.Show("Sure you want to delete character " & SelectedChar & "?", "You sure?", MessageBoxButtons.YesNo)
        If result1 = DialogResult.Yes Then
            Dim buffer As New ByteStream(4)
            buffer.WriteInt32(ClientPacket.CDelChar)
            buffer.WriteInt32(SelectedChar)
            Network.SendData(buffer.ToPacket)
            buffer.Dispose()
        End If
    End Sub

#End Region

End Class