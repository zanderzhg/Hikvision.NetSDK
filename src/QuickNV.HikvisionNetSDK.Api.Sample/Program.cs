﻿using QuickNV.HikvisionNetSDK.Api;
using Newtonsoft.Json;
using System.Text;

var host = "127.0.0.1";
var port = 8000;
var user = "admin";
var password = "123456";
var encoding = CodePagesEncodingProvider.Instance.GetEncoding("gb2312");

try
{
    HvSession.Init();
    Console.WriteLine("初始化成功！");
    //登录
    Console.WriteLine("正在登录...");
    var session = HvSession.Login(host, port, user, password, encoding);
    Console.WriteLine("登录成功！");
    Console.WriteLine("设备时间: " + session.ConfigService.GetTime());
    Console.WriteLine("设备配置: " + JsonConvert.SerializeObject(session.ConfigService.GetDeviceConfig(), Formatting.Indented));
    Console.WriteLine("网络配置: " + JsonConvert.SerializeObject(session.ConfigService.GetNetworkConfig(), Formatting.Indented));
    Console.WriteLine("硬盘配置: " + JsonConvert.SerializeObject(session.ConfigService.GetHdConfigs(), Formatting.Indented));
    Console.WriteLine("正在读取通道名称...");
    session.ChannelService.RefreshChannelsName();
    Console.WriteLine("模拟通道：" + string.Join(",", session.ChannelService.AnalogChannels.Select(t => $"通道{t.Id}_{t.Name}")));
    Console.WriteLine("IP通道：" + string.Join(",", session.ChannelService.IpChannels.Select(t => $"通道{t.Id}_{t.Name}")));

    //获取RTSP地址
    var rtsp = session.ChannelService.GetRtspUrl(session.ChannelService.AllChannels.First(), HvStreamType.Main);
    //获取PTZ参数
    var ptzPosition = session.ChannelService.GetPtzPosition(session.ChannelService.AllChannels.First().Id);

    //设置当前位置为预置位并命名
    //session.ChannelService.SetPresetWithName(33, 5, "SDK测试-50");
    //session.ChannelService.SetPresetWithName(1, 6, "SDK测试-6");
    //转动到指定预置位
    //session.ChannelService.GotoPreset(33, 5);
    //删除预置位
    //session.ChannelService.RemovePreset(33, 6);
    //直到摄像机停止转动
    //await session.ChannelService.WaitUntilStopMovingAsync(33);

    //输出所有有效预置位
    foreach (var channel in session.ChannelService.AllChannels)
    {
        Console.WriteLine($"通道 [{channel.Name}] 预置位信息：");
        var presets = session.ChannelService.GetPresets(channel.Id);
        foreach (var preset in presets)
        {
            Console.WriteLine(preset);
        }
    }

    //for (int i = 1; i <= 100; i += 2)
    //{
    //    var path1 = $"images/{i}.jpg";
    //    var path2 = $"images/{i + 1}.jpg";

    //    // 抓图方式1：
    //    session.PictureService.CaptureJPEGPicture(session.ChannelService.AllChannels.First().Id, path1);

    //    // 抓图方式2：
    //    var pictureBytes = session.PictureService.CaptureJPEGPicture_NEW(session.ChannelService.AllChannels.First().Id);
    //    File.WriteAllBytes(path2, pictureBytes);
    //}

    ////如果存在IP通道
    //if (session.ChannelService.IpChannels.Count > 0)
    //{
    //    Console.WriteLine("开始查找录像文件...");
    //    var channel = session.ChannelService.IpChannels.First();
    //    var stopTime = DateTime.Now;
    //    var startTime = stopTime.AddDays(-1);
    //    var files = session.VideoFileService.FindFile(channel.Id, startTime, stopTime);
    //    Console.WriteLine("录像文件：" + JsonConvert.SerializeObject(files, Formatting.Indented));
    //    Console.WriteLine("查找录像文件完成.");


    //    Console.WriteLine("开始下载...");
    //    //var channel = session.ChannelService.IpChannels.First();
    //    //var stopTime = DateTime.Now;
    //    //var startTime = stopTime.AddMinutes(-1);
    //    var localFileName = $"{channel.Name}_{startTime.ToString("yyyyMMddHHmmss")}_{stopTime.ToString("yyyyMMddHHmmss")}.mp4";
    //    var fileHandle = session.VideoFileService.StartDownloadFile(channel.Id, startTime, stopTime, localFileName);
    //    while (true)
    //    {
    //        Thread.Sleep(1000);
    //        var progress = session.VideoFileService.GetDownloadPosition(fileHandle);
    //        if (progress < 0 || progress > 100)
    //            throw new Exception("Download error.Progress: " + progress);
    //        Console.CursorLeft = 0;
    //        Console.Write($"进度：{progress}%");
    //        if (progress == 100)
    //            break;
    //    }
    //    Console.WriteLine();
    //    session.VideoFileService.StopDownloadFile(fileHandle);
    //    Console.WriteLine("下载完成。文件：" + localFileName);

    //}
    //退出登录
    session.Logout();
    Console.WriteLine("退出登录完成");

    //清理
    HvSession.Cleanup();
    Console.WriteLine("清理完成");

    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}