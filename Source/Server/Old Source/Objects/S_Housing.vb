Imports System
Imports System.IO
Imports ASFW
Imports ASFW.IO.FileIO

Friend Module S_Housing

#Region "Globals & Types"

    Friend MAX_HOUSES As Integer = 100

    Friend HouseConfig() As HouseRec

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
        Dim Houseindex As Integer
        Dim FurnitureCount As Integer
        Dim Furniture() As FurnitureRec
    End Structure

#End Region

#Region "DataBase"

    Sub CheckHouses()
        Dim cd = Environment.CurrentDirectory & "\Data\Houses\"
        If Not Directory.Exists(cd) Then Directory.CreateDirectory(cd)

        For i = 1 To MAX_HOUSES

            If Not File.Exists(cd & i & "bin") Then
                SaveAnimation(i)
            End If

        Next
    End Sub

    Sub LoadHouse(housenum As Integer)
        Dim cf = Environment.CurrentDirectory & "\Data\Houses\" & housenum & "bin"

        Dim reader As New ByteStream()
        BinaryFile.Load(cf, reader)

        HouseConfig(housenum).ConfigName = reader.ReadString()
        HouseConfig(housenum).BaseMap = reader.ReadInt32()
        HouseConfig(housenum).MaxFurniture = reader.ReadInt32()
        HouseConfig(housenum).Price = reader.ReadInt32()
        HouseConfig(housenum).X = reader.ReadInt32()
        HouseConfig(housenum).Y = reader.ReadInt32()
        HouseConfig(housenum).BaseMap = reader.ReadInt32()

        If HouseConfig(housenum).ConfigName Is Nothing Then
            HouseConfig(housenum).ConfigName = ""
        End If

        For i = 1 To GetPlayersOnline()
            If Network.IsPlaying(i) Then
                SendHouseConfigs(i)
            End If
        Next

    End Sub

    Sub LoadHouses()
        Dim cd = Environment.CurrentDirectory & "\Data\Houses\"
        If Not Directory.Exists(cd) Then Directory.CreateDirectory(cd)

        CheckHouses()

        For i = 1 To MAX_ANIMATIONS
            LoadHouse(i)
        Next
    End Sub

    Sub SaveHouse(housenum As Integer)
        Dim cf = Environment.CurrentDirectory & "\Data\Houses\" & housenum & "bin"

        Dim writer As New ByteStream()
        BinaryFile.Load(cf, writer)

        writer.WriteString(HouseConfig(housenum).ConfigName)
        writer.WriteInt32(HouseConfig(housenum).BaseMap)
        writer.WriteInt32(HouseConfig(housenum).MaxFurniture)
        writer.WriteInt32(HouseConfig(housenum).Price)
        writer.WriteInt32(HouseConfig(housenum).X)
        writer.WriteInt32(HouseConfig(housenum).Y)
        writer.WriteInt32(HouseConfig(housenum).BaseMap)

        BinaryFile.Save(cf, writer)

    End Sub

    Sub SaveHouses()
        Dim i As Integer

        For i = 1 To MAX_HOUSES
            SaveHouse(i)
        Next

    End Sub

#End Region

#Region "Incoming Packets"

    Sub Packet_BuyHouse(index As Integer, ByRef data() As Byte)
        Dim i As Integer, price As Integer
        Dim buffer As New ByteStream(data)
        i = buffer.ReadInt32

        If i = 1 Then
            If TempPlayer(index).BuyHouseindex > 0 Then
                price = HouseConfig(TempPlayer(index).BuyHouseindex).Price
                If HasItem(index, 1) >= price Then
                    TakeInvItem(index, 1, price)
                    Player(index).Character(TempPlayer(index).CurChar).House.Houseindex = TempPlayer(index).BuyHouseindex
                    Network.PlayerMsg(index, "You just bought the " & HouseConfig(TempPlayer(index).BuyHouseindex).ConfigName.Trim & " house!", ColorType.BrightGreen)
                    Player(index).Character(TempPlayer(index).CurChar).LastMap = GetPlayerMap(index)
                    Player(index).Character(TempPlayer(index).CurChar).LastX = GetPlayerX(index)
                    Player(index).Character(TempPlayer(index).CurChar).LastY = GetPlayerY(index)
                    Player(index).Character(TempPlayer(index).CurChar).InHouse = index

                    PlayerWarp(index, HouseConfig(Player(index).Character(TempPlayer(index).CurChar).House.Houseindex).BaseMap, HouseConfig(Player(index).Character(TempPlayer(index).CurChar).House.Houseindex).X, HouseConfig(Player(index).Character(TempPlayer(index).CurChar).House.Houseindex).Y, True)
                    SavePlayer(index)
                Else
                    Network.PlayerMsg(index, "You cannot afford this house!", ColorType.BrightRed)
                End If
            End If
        End If

        TempPlayer(index).BuyHouseindex = 0

        buffer.Dispose()

    End Sub

    Sub Packet_InviteToHouse(index As Integer, ByRef data() As Byte)
        Dim invitee As Integer, Name As String
        Dim buffer As New ByteStream(data)
        Name = buffer.ReadString.Trim
        invitee = FindPlayer(Name)
        buffer.Dispose()

        If invitee = 0 Then
            Network.PlayerMsg(index, "Player not found.", ColorType.BrightRed)
            Exit Sub
        End If

        If index = invitee Then
            Network.PlayerMsg(index, "You cannot invite yourself to you own house!", ColorType.BrightRed)
            Exit Sub
        End If

        If TempPlayer(invitee).Invitationindex > 0 Then
            If TempPlayer(invitee).InvitationTimer > GetTimeMs() Then
                Network.PlayerMsg(index, GetPlayerName(invitee).Trim & " is currently busy!", ColorType.Yellow)
                Exit Sub
            End If
        End If

        If Player(index).Character(TempPlayer(index).CurChar).House.Houseindex > 0 Then
            If Player(index).Character(TempPlayer(index).CurChar).InHouse > 0 Then
                If Player(index).Character(TempPlayer(index).CurChar).InHouse = index Then
                    If Player(invitee).Character(TempPlayer(invitee).CurChar).InHouse > 0 Then
                        If Player(invitee).Character(TempPlayer(invitee).CurChar).InHouse = index Then
                            Network.PlayerMsg(index, GetPlayerName(invitee).Trim & " is already in your house!", ColorType.Yellow)
                        Else
                            Network.PlayerMsg(index, GetPlayerName(invitee).Trim & " is already visiting someone elses house!", ColorType.Yellow)
                        End If
                    Else
                        'Send invite
                        buffer = New ByteStream(4)
                        buffer.WriteInt32(ServerPacket.SVisit)
                        buffer.WriteInt32(index)
                        Network.SendDataTo(invitee, buffer.ToPacket)
                        TempPlayer(invitee).Invitationindex = index
                        TempPlayer(invitee).InvitationTimer = GetTimeMs() + 15000
                        buffer.Dispose()
                    End If
                Else
                    Network.PlayerMsg(index, "Only the house owner can invite other players into their house.", ColorType.BrightRed)
                End If
            Else
                Network.PlayerMsg(index, "You must be inside your house before you can invite someone to visit!", ColorType.BrightRed)
            End If
        Else
            Network.PlayerMsg(index, "You do not have a house to invite anyone to!", ColorType.BrightRed)
        End If

    End Sub

    Sub Packet_AcceptInvite(index As Integer, ByRef data() As Byte)
        Dim response As Integer
        Dim buffer As New ByteStream(data)
        response = buffer.ReadInt32
        buffer.Dispose()

        If response = 1 Then
            If TempPlayer(index).Invitationindex > 0 Then
                If TempPlayer(index).InvitationTimer > GetTimeMs() Then
                    'Accept this invite
                    If Network.IsPlaying(TempPlayer(index).Invitationindex) Then
                        Player(index).Character(TempPlayer(index).CurChar).InHouse = TempPlayer(index).Invitationindex
                        Player(index).Character(TempPlayer(index).CurChar).LastX = GetPlayerX(index)
                        Player(index).Character(TempPlayer(index).CurChar).LastY = GetPlayerY(index)
                        Player(index).Character(TempPlayer(index).CurChar).LastMap = GetPlayerMap(index)
                        TempPlayer(index).InvitationTimer = 0
                        PlayerWarp(index, Player(TempPlayer(index).Invitationindex).Character(TempPlayer(index).CurChar).Map, HouseConfig(Player(TempPlayer(index).Invitationindex).Character(TempPlayer(TempPlayer(index).Invitationindex).CurChar).House.Houseindex).X, HouseConfig(Player(TempPlayer(index).Invitationindex).Character(TempPlayer(TempPlayer(index).Invitationindex).CurChar).House.Houseindex).Y, True, True)
                    Else
                        TempPlayer(index).InvitationTimer = 0
                        Network.PlayerMsg(index, "Cannot find player!", ColorType.BrightRed)
                    End If
                Else
                    Network.PlayerMsg(index, "Your invitation has expired, have your friend re-invite you.", ColorType.Yellow)
                End If
            End If
        Else
            If Network.IsPlaying(TempPlayer(index).Invitationindex) Then
                TempPlayer(index).InvitationTimer = 0
                Network.PlayerMsg(TempPlayer(index).Invitationindex, GetPlayerName(index).Trim & " rejected your invitation", ColorType.BrightRed)
            End If
        End If

    End Sub

    Sub Packet_PlaceFurniture(index As Integer, ByRef data() As Byte)
        Dim i As Integer, x As Integer, y As Integer, invslot As Integer
        Dim ItemNum As Integer, x1 As Integer, y1 As Integer, widthoffset As Integer

        Dim buffer As New ByteStream(data)
        x = buffer.ReadInt32
        y = buffer.ReadInt32
        invslot = buffer.ReadInt32
        buffer.Dispose()

        ItemNum = Player(index).Character(TempPlayer(index).CurChar).Inv(invslot).Num

        ' Prevent hacking
        If ItemNum < 1 OrElse ItemNum > MAX_ITEMS Then Exit Sub

        If Player(index).Character(TempPlayer(index).CurChar).InHouse = index Then
            If Item(ItemNum).Type = ItemType.Furniture Then
                ' stat requirements
                For i = 1 To StatType.Count - 1
                    If GetPlayerRawStat(index, i) < Item(ItemNum).Stat_Req(i) Then
                        Network.PlayerMsg(index, "You do not meet the stat requirements to use this item.", ColorType.BrightRed)
                        Exit Sub
                    End If
                Next

                ' level requirement
                If GetPlayerLevel(index) < Item(ItemNum).LevelReq Then
                    Network.PlayerMsg(index, "You do not meet the level requirement to use this item.", ColorType.BrightRed)
                    Exit Sub
                End If

                ' class requirement
                If Item(ItemNum).ClassReq > 0 Then
                    If Not GetPlayerClass(index) = Item(ItemNum).ClassReq Then
                        Network.PlayerMsg(index, "You do not meet the class requirement to use this item.", ColorType.BrightRed)
                        Exit Sub
                    End If
                End If

                ' access requirement
                If Not GetPlayerAccess(index) >= Item(ItemNum).AccessReq Then
                    Network.PlayerMsg(index, "You do not meet the access requirement to use this item.", ColorType.BrightRed)
                    Exit Sub
                End If

                'Ok, now we got to see what can be done about this furniture :/
                If Player(index).Character(TempPlayer(index).CurChar).InHouse <> index Then
                    Network.PlayerMsg(index, "You must be inside your house to place furniture!", ColorType.Yellow)
                    Exit Sub
                End If

                If Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount >= HouseConfig(Player(index).Character(TempPlayer(index).CurChar).House.Houseindex).MaxFurniture Then
                    If HouseConfig(Player(index).Character(TempPlayer(index).CurChar).House.Houseindex).MaxFurniture > 0 Then
                        Network.PlayerMsg(index, "Your house cannot hold any more furniture!", ColorType.BrightRed)
                        Exit Sub
                    End If
                End If

                If x < 0 OrElse x > Map(GetPlayerMap(index)).MaxX Then Exit Sub
                If y < 0 OrElse y > Map(GetPlayerMap(index)).MaxY Then Exit Sub

                If Item(ItemNum).FurnitureWidth > 2 Then
                    x1 = x + (Item(ItemNum).FurnitureWidth / 2)
                    widthoffset = x1 - x
                    x1 = x1 - (Item(ItemNum).FurnitureWidth - widthoffset)
                Else
                    x1 = x
                End If

                x1 = x
                widthoffset = 0

                y1 = y

                If widthoffset > 0 Then

                    For x = x1 To x1 + widthoffset
                        For y = y1 To y1 - Item(ItemNum).FurnitureHeight + 1 Step -1
                            If Map(GetPlayerMap(index)).Tile(x, y).Type = TileType.Blocked Then Exit Sub

                            For i = 1 To GetPlayersOnline()
                                If Network.IsPlaying(i) AndAlso i <> index AndAlso GetPlayerMap(i) = GetPlayerMap(index) Then
                                    If Player(i).Character(TempPlayer(i).CurChar).InHouse = Player(index).Character(TempPlayer(index).CurChar).InHouse Then
                                        If Player(i).Character(TempPlayer(i).CurChar).X = x AndAlso Player(i).Character(TempPlayer(i).CurChar).Y = y Then
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Next

                            If Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount > 0 Then
                                For i = 1 To Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount
                                    If x >= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).X AndAlso x <= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).X + Item(Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).ItemNum).FurnitureWidth - 1 Then
                                        If y <= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).Y AndAlso y >= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).Y - Item(Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).ItemNum).FurnitureHeight + 1 Then
                                            'Blocked!
                                            Exit Sub
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    Next

                    For x = x1 To x1 - (Item(ItemNum).FurnitureWidth - widthoffset) Step -1
                        For y = y1 To y1 - Item(ItemNum).FurnitureHeight + 1 Step -1
                            If Map(GetPlayerMap(index)).Tile(x, y).Type = TileType.Blocked Then Exit Sub

                            For i = 1 To GetPlayersOnline()
                                If Network.IsPlaying(i) AndAlso i <> index AndAlso GetPlayerMap(i) = GetPlayerMap(index) Then
                                    If Player(i).Character(TempPlayer(i).CurChar).InHouse = Player(index).Character(TempPlayer(index).CurChar).InHouse Then
                                        If Player(i).Character(TempPlayer(i).CurChar).X = x AndAlso Player(i).Character(TempPlayer(i).CurChar).Y = y Then
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Next

                            If Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount > 0 Then
                                For i = 1 To Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount
                                    If x >= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).X AndAlso x <= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).X + Item(Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).ItemNum).FurnitureWidth - 1 Then
                                        If y <= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).Y AndAlso y >= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).Y - Item(Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).ItemNum).FurnitureHeight + 1 Then
                                            'Blocked!
                                            Exit Sub
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    Next
                Else
                    For x = x1 To x1 + Item(ItemNum).FurnitureWidth - 1
                        For y = y1 To y1 - Item(ItemNum).FurnitureHeight + 1 Step -1
                            If Map(GetPlayerMap(index)).Tile(x, y).Type = TileType.Blocked Then Exit Sub

                            For i = 1 To GetPlayersOnline()
                                If Network.IsPlaying(i) AndAlso i <> index AndAlso GetPlayerMap(i) = GetPlayerMap(index) Then
                                    If Player(i).Character(TempPlayer(i).CurChar).InHouse = Player(index).Character(TempPlayer(index).CurChar).InHouse Then
                                        If Player(i).Character(TempPlayer(i).CurChar).X = x AndAlso Player(i).Character(TempPlayer(i).CurChar).Y = y Then
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Next

                            If Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount > 0 Then
                                For i = 1 To Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount
                                    If x >= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).X AndAlso x <= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).X + Item(Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).ItemNum).FurnitureWidth - 1 Then
                                        If y <= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).Y AndAlso y >= Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).Y - Item(Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).ItemNum).FurnitureHeight + 1 Then
                                            'Blocked!
                                            Exit Sub
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    Next
                End If

                x = x1
                y = y1

                'If all checks out, place furniture and send the update to everyone in the player's house.
                Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount = Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount + 1
                ReDim Preserve Player(index).Character(TempPlayer(index).CurChar).House.Furniture(Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount)
                Player(index).Character(TempPlayer(index).CurChar).House.Furniture(Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount).ItemNum = ItemNum
                Player(index).Character(TempPlayer(index).CurChar).House.Furniture(Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount).X = x
                Player(index).Character(TempPlayer(index).CurChar).House.Furniture(Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount).Y = y

                TakeInvItem(index, ItemNum, 0)

                SendFurnitureToHouse(Player(index).Character(TempPlayer(index).CurChar).InHouse)

                SavePlayer(index)
            End If
        Else
            Network.PlayerMsg(index, "You cannot place furniture unless you are in your own house!", ColorType.BrightRed)
        End If

    End Sub

    Sub Packet_RequestEditHouse(index As Integer, ByRef data() As Byte)
        Dim buffer As ByteStream, i As Integer

        ' Prevent hacking
        If GetPlayerAccess(index) < AdminType.Mapper Then Exit Sub

        buffer = New ByteStream(4)
        buffer.WriteInt32(ServerPacket.SHouseEdit)
        For i = 1 To MAX_HOUSES
            buffer.WriteString((HouseConfig(i).ConfigName.Trim))
            buffer.WriteInt32(HouseConfig(i).BaseMap)
            buffer.WriteInt32(HouseConfig(i).X)
            buffer.WriteInt32(HouseConfig(i).Y)
            buffer.WriteInt32(HouseConfig(i).Price)
            buffer.WriteInt32(HouseConfig(i).MaxFurniture)
        Next
        Network.SendDataTo(index, buffer.ToPacket)
        buffer.Dispose()

    End Sub

    Sub Packet_SaveHouses(index As Integer, ByRef data() As Byte)
        Dim i As Integer, x As Integer, Count As Integer, z As Integer

        ' Prevent hacking
        If GetPlayerAccess(index) < AdminType.Mapper Then Exit Sub

        Dim buffer As New ByteStream(data)
        Count = buffer.ReadInt32
        If Count > 0 Then
            For z = 1 To Count
                i = buffer.ReadInt32
                HouseConfig(i).ConfigName = buffer.ReadString.Trim
                HouseConfig(i).BaseMap = buffer.ReadInt32
                HouseConfig(i).X = buffer.ReadInt32
                HouseConfig(i).Y = buffer.ReadInt32
                HouseConfig(i).Price = buffer.ReadInt32
                HouseConfig(i).MaxFurniture = buffer.ReadInt32
                SaveHouse(i)

                For x = 1 To GetPlayersOnline()
                    If Network.IsPlaying(x) AndAlso Player(x).Character(TempPlayer(x).CurChar).InHouse = i Then
                        SendFurnitureToHouse(i)
                    End If
                Next
            Next
        End If

        buffer.Dispose()

    End Sub

    Sub Packet_SellHouse(index As Integer, ByRef data() As Byte)
        Dim i As Integer, refund As Integer
        Dim Tmpindex As Integer
        Dim buffer As New ByteStream(data)
        Tmpindex = Player(index).Character(TempPlayer(index).CurChar).House.Houseindex
        If Tmpindex > 0 Then
            'get some money back
            refund = HouseConfig(Tmpindex).Price / 2

            Player(index).Character(TempPlayer(index).CurChar).House.Houseindex = 0
            Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount = 0
            ReDim Player(index).Character(TempPlayer(index).CurChar).House.Furniture(Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount)

            For i = 0 To Player(index).Character(TempPlayer(index).CurChar).House.FurnitureCount
                Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).ItemNum = 0
                Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).X = 0
                Player(index).Character(TempPlayer(index).CurChar).House.Furniture(i).Y = 0
            Next

            If Player(index).Character(TempPlayer(index).CurChar).InHouse = Tmpindex Then
                PlayerWarp(index, Player(index).Character(TempPlayer(index).CurChar).LastMap, Player(index).Character(TempPlayer(index).CurChar).LastX, Player(index).Character(TempPlayer(index).CurChar).LastY)
            End If

            SavePlayer(index)

            Network.PlayerMsg(index, "You sold your House for " & refund & " Gold!", ColorType.BrightGreen)
            GiveInvItem(index, 1, refund)
        Else
            Network.PlayerMsg(index, "You dont own a House!", ColorType.BrightRed)
        End If

        buffer.Dispose()

    End Sub

#End Region

#Region "OutGoing Packets"

    Sub SendHouseConfigs(index As Integer)
        Dim buffer As New ByteStream(4), i As Integer

        buffer.WriteInt32(ServerPacket.SHouseConfigs)

        For i = 1 To MAX_HOUSES
            buffer.WriteString((HouseConfig(i).ConfigName.Trim))
            buffer.WriteInt32(HouseConfig(i).BaseMap)
            buffer.WriteInt32(HouseConfig(i).MaxFurniture)
            buffer.WriteInt32(HouseConfig(i).Price)
        Next

        Network.SendDataTo(index, buffer.ToPacket)
        buffer.Dispose()

    End Sub

    Sub SendFurnitureToHouse(Houseindex As Integer)
        Dim buffer As New ByteStream(4), i As Integer

        buffer.WriteInt32(ServerPacket.SFurniture)
        buffer.WriteInt32(Houseindex)
        buffer.WriteInt32(Player(Houseindex).Character(TempPlayer(Houseindex).CurChar).House.FurnitureCount)
        If Player(Houseindex).Character(TempPlayer(Houseindex).CurChar).House.FurnitureCount > 0 Then
            For i = 1 To Player(Houseindex).Character(TempPlayer(Houseindex).CurChar).House.FurnitureCount
                buffer.WriteInt32(Player(Houseindex).Character(TempPlayer(Houseindex).CurChar).House.Furniture(i).ItemNum)
                buffer.WriteInt32(Player(Houseindex).Character(TempPlayer(Houseindex).CurChar).House.Furniture(i).X)
                buffer.WriteInt32(Player(Houseindex).Character(TempPlayer(Houseindex).CurChar).House.Furniture(i).Y)
            Next
        End If

        For i = 1 To GetPlayersOnline()
            If Network.IsPlaying(i) Then
                If Player(i).Character(TempPlayer(i).CurChar).InHouse = Houseindex Then
                    Network.SendDataTo(i, buffer.ToPacket)
                End If
            End If
        Next

        buffer.Dispose()

    End Sub

#End Region

End Module