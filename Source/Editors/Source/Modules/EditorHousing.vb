﻿Imports ASFW
Imports SFML.Graphics
Imports SFML.Window

Public Module EditorHousing

#Region "Globals & Types"
    Public MAX_HOUSES As Integer = 100

    Public FurnitureCount As Integer
    Public FurnitureHouse As Integer
    Public FurnitureSelected As Integer
    Public HouseTileIndex As Integer

    Public House() As HouseRec
    Public HouseConfig() As HouseRec
    Public Furniture() As FurnitureRec
    Public NumFurniture As Integer
    Public House_Changed(0 To MAX_HOUSES) As Boolean
    Public HouseEdit As Boolean

    Structure HouseRec
        Dim ConfigName As String
        Dim BaseMap As Integer
        Dim Price As Integer
        Dim MaxFurniture As Integer
        Dim X As Integer
        Dim Y As Integer
    End Structure

    Structure FurnitureRec
        Dim ItemNum As Integer
        Dim X As Integer
        Dim Y As Integer
    End Structure

    Structure PlayerHouseRec
        Dim HouseIndex As Integer
        Dim FurnitureCount As Integer
        Dim Furniture() As FurnitureRec
    End Structure
#End Region

#Region "Incoming Packets"
    Sub Packet_HouseConfigurations(ByRef Data() As Byte)
        Dim i As Integer
        Dim Buffer As New ByteStream(Data)
        For i = 1 To MAX_HOUSES
            HouseConfig(i).ConfigName = Buffer.ReadString
            HouseConfig(i).BaseMap = Buffer.ReadInt32
            HouseConfig(i).MaxFurniture = Buffer.ReadInt32
            HouseConfig(i).Price = Buffer.ReadInt32
        Next
        Buffer.Dispose()

    End Sub

    Sub Packet_Furniture(ByRef Data() As Byte)
        Dim i As Integer
        Dim Buffer As New ByteStream(Data)
        FurnitureHouse = Buffer.ReadInt32
        FurnitureCount = Buffer.ReadInt32

        ReDim Furniture(FurnitureCount)
        If FurnitureCount > 0 Then
            For i = 1 To FurnitureCount
                Furniture(i).ItemNum = Buffer.ReadInt32
                Furniture(i).X = Buffer.ReadInt32
                Furniture(i).Y = Buffer.ReadInt32
            Next
        End If

        Buffer.Dispose()

    End Sub

    Sub Packet_EditHouses(ByRef Data() As Byte)
        Dim i As Integer
        Dim Buffer As New ByteStream(Data)
        For i = 1 To MAX_HOUSES
            With House(i)
                .ConfigName = Trim$(Buffer.ReadString)
                .BaseMap = Buffer.ReadInt32
                .X = Buffer.ReadInt32
                .Y = Buffer.ReadInt32
                .Price = Buffer.ReadInt32
                .MaxFurniture = Buffer.ReadInt32
            End With
        Next

        HouseEdit = True

        Buffer.Dispose()

    End Sub
#End Region

#Region "Outgoing Packets"
    Public Sub SendRequestEditHouse()
        Dim Buffer As ByteStream

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditHouse)
        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()

    End Sub

    Public Sub SendBuyHouse(ByVal Accepted As Byte)
        Dim Buffer As ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CBuyHouse)
        Buffer.WriteInt32(Accepted)
        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Public Sub SendInvite(ByVal Name As String)
        Dim Buffer As ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CVisit)
        Buffer.WriteString(Name)
        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Public Sub SendVisit(ByVal Accepted As Byte)
        Dim Buffer As ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CAcceptVisit)
        Buffer.WriteInt32(Accepted)
        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub
#End Region

#Region "Editor"
    Public Sub HouseEditorInit()

        If FrmEditor_House.Visible = False Then Exit Sub

        EditorIndex = FrmEditor_House.lstIndex.SelectedIndex + 1

        With House(EditorIndex)
            FrmEditor_House.txtName.Text = Trim$(.ConfigName)
            If .BaseMap = 0 Then .BaseMap = 1
            FrmEditor_House.nudBaseMap.Value = .BaseMap
            If .X = 0 Then .X = 1
            FrmEditor_House.nudX.Value = .X
            If .Y = 0 Then .Y = 1
            FrmEditor_House.nudY.Value = .Y
            FrmEditor_House.nudPrice.Value = .Price
            FrmEditor_House.nudFurniture.Value = .MaxFurniture
        End With

        House_Changed(EditorIndex) = True

    End Sub

    Public Sub HouseEditorCancel()

        Editor = 0
        FrmEditor_House.Dispose()

        ClearChanged_House()

    End Sub

    Public Sub HouseEditorOk()
        Dim i As Integer, Buffer As ByteStream, count As Integer
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveHouses)

        For i = 1 To MAX_HOUSES
            If House_Changed(i) Then count = count + 1
        Next

        Buffer.WriteInt32(count)

        If count > 0 Then
            For i = 1 To MAX_HOUSES
                If House_Changed(i) Then
                    Buffer.WriteInt32(i)
                    Buffer.WriteString(Trim$(House(i).ConfigName))
                    Buffer.WriteInt32(House(i).BaseMap)
                    Buffer.WriteInt32(House(i).X)
                    Buffer.WriteInt32(House(i).Y)
                    Buffer.WriteInt32(House(i).Price)
                    Buffer.WriteInt32(House(i).MaxFurniture)
                End If
            Next
        End If

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose
        FrmEditor_House.Dispose()
        Editor = 0

        ClearChanged_House()

    End Sub

    Public Sub ClearChanged_House()

        For i = 1 To MAX_HOUSES
            House_Changed(i) = Nothing
        Next i

        ReDim House_Changed(0 To MAX_HOUSES)
    End Sub

#End Region

#Region "Drawing"
    Public Sub CheckFurniture()
        Dim i As Integer
        i = 1

        While FileExist(Application.StartupPath & GFX_PATH & "Furniture\" & i & GFX_EXT)
            NumFurniture = NumFurniture + 1
            i = i + 1
        End While

        If NumFurniture = 0 Then Exit Sub
    End Sub

    Public Sub DrawFurniture(ByVal Index As Integer, Layer As Integer)
        Dim i As Integer, ItemNum As Integer
        Dim X As Integer, Y As Integer, Width As Integer, Height As Integer, X1 As Integer, Y1 As Integer

        ItemNum = Furniture(Index).ItemNum

        If Item(ItemNum).Type <> ItemType.Furniture Then Exit Sub

        i = Item(ItemNum).Data2

        If FurnitureGFXInfo(i).IsLoaded = False Then
            LoadTexture(i, 10)
        End If

        'seeying we still use it, lets update timer
        With SkillIconsGFXInfo(i)
            .TextureTimer = GetTickCount() + 100000
        End With

        Width = Item(ItemNum).FurnitureWidth
        Height = Item(ItemNum).FurnitureHeight

        If Width > 4 Then Width = 4
        If Height > 4 Then Height = 4
        If i <= 0 Or i > NumFurniture Then Exit Sub

        ' make sure it's not out of map
        If Furniture(Index).X > Map.MaxX Then Exit Sub
        If Furniture(Index).Y > Map.MaxY Then Exit Sub

        For X1 = 0 To Width - 1
            For Y1 = 0 To Height
                If Item(Furniture(Index).ItemNum).FurnitureFringe(X1, Y1) = Layer Then
                    ' Set base x + y, then the offset due to size
                    X = (Furniture(Index).X * 32) + (X1 * 32)
                    Y = (Furniture(Index).Y * 32 - (Height * 32)) + (Y1 * 32)
                    X = ConvertMapX(X)
                    Y = ConvertMapY(Y)

                    Dim tmpSprite As Sprite = New Sprite(FurnitureGFX(i))
                    tmpSprite.TextureRect = New IntRect(0 + (X1 * 32), 0 + (Y1 * 32), 32, 32)
                    tmpSprite.Position = New Vector2f(X, Y)
                    GameWindow.Draw(tmpSprite)
                End If
            Next
        Next

    End Sub
#End Region

End Module
