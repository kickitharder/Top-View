Imports AForge
Imports AForge.Video
Imports AForge.Video.DirectShow

Public Class Form1
    Dim FormName = "Top View (231129)"
    Dim Camera As VideoCaptureDevice
    Dim CamNo As Integer = -1
    Dim FrameRate As Integer = 1
    'Dim VideoCaptureDevice As VideoCaptureDevice
    Dim Bmp As Bitmap

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Startup()
    End Sub

    Private Sub Startup()
        Dim cameras As New VideoCaptureDeviceForm
        Dim videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)
        Me.Text = FormName
        CamNo = -1

        For n = 0 To videoDevices.Count - 1
            If videoDevices(n).MonikerString.Contains("global") Then CamNo = n
        Next

        If CamNo >= 0 Then
            Try
                Camera = New VideoCaptureDevice(videoDevices(CamNo).MonikerString)
                AddHandler Camera.NewFrame, New NewFrameEventHandler(AddressOf Captured)
                Camera.Start()
                FrameRate = 1
            Catch ex As Exception
                MsgBox("Unable to activate camera:" + vbCr + vbCr + videoDevices(CamNo).MonikerString)
            End Try
        Else
            MsgBox("Camera not available.")
        End If
    End Sub

    Private Sub Captured(ByVal sender As Object, ByVal eventArgs As NewFrameEventArgs)
        If Not Me.Visible Then Exit Sub
        FrameRate -= 1
        If FrameRate Then Exit Sub
        Me.TopMost = (Me.Height = 184)
        PbxView.Image = DirectCast(eventArgs.Frame.Clone(), Bitmap)
        PbxView.SizeMode = PictureBoxSizeMode.StretchImage
        FrameRate = 1
    End Sub

    'eventArgs.Frame.Dispose()
    'Bmp = DirectCast(eventArgs.Frame.Clone(), Bitmap)
    'PictureBox1.Image = DirectCast(eventArgs.Frame.Clone(), Bitmap)

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        StopCamera()
    End Sub

    Private Sub StopCamera()
        If CamNo >= 0 Then
            If Camera.IsRunning Then
                Camera.SignalToStop()
                Camera.WaitForStop()
                Camera.Stop()
            End If
        End If
    End Sub

    Private Sub PbxView_DoubleClick(sender As Object, e As EventArgs) Handles PbxView.DoubleClick
        Dim cameras As VideoCaptureDeviceForm = New VideoCaptureDeviceForm
        StopCamera()
        Me.Visible = False
        If cameras.ShowDialog = Windows.Forms.DialogResult.OK Then
            Camera = cameras.VideoDevice
            AddHandler Camera.NewFrame, New NewFrameEventHandler(AddressOf Captured)
            Camera.Start()
            CamNo = 1
        End If
        Me.Visible = True
    End Sub
End Class
