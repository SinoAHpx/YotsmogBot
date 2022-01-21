using AHpx.Extensions.StringExtensions;
using Mirai.Net.Data.Messages.Concretes;

var m1 = new PlainMessage("awdawd").ToJsonString();
var m2 = new PlainMessage("awdawd").ToJsonString();

Console.WriteLine(m1 == m2);