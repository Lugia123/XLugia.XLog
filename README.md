#XLugia.XLog
###简介

>     XLugia.XLog是一款.Net下的高速文件型日志引擎，轻巧易用，并配有日志查看工具，方便快速查看GB级的大型日志。 	
>     开发该款日志引擎的初衷是为了替代原文本型日志速度不够快，并且日志过大后不方便查看的缺点。
>     该日志引擎使用方便，无需配置，直接调用接口即可，并配有日志查看工具，方便查看日志。
>     本引擎使用C#编写，需要.Net framework 4.0。
>     使用上有问题可以联系我。
>     邮件:watarux@qq.com
>     QQ:56809958    
>     交流群:334533178

###更新履历
####2015-06-18
>1.初次版本发布。

###使用方法
####1.写入日志
```javascript
    LogWriter.getIns().writeLog("日志内容", LogType.getIns().debug.application);
```

