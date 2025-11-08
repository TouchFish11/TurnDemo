using System.Collections;

namespace Framework
{
    /*
        示例脚本：如何启动框架，完成最基本的游戏初始化工作。
    */

    /// <summary>
    /// 游戏主函数入口
    /// </summary>
    public class Main : SingletonMono<Main>
    {
        //记录下载阶段
        //private E_UpdatePhase phase = E_UpdatePhase.None;
        //当前下载字节数
        //private ulong nowDownLoadSize = 0;
        //当前帧下载字节数为0的次数
        //private int nowNum = 0;
        //最大帧下载字节数为0的次数
        //private int maxNum = 1200;

        void Start()
        {
            //LogMgr.Log(Application.persistentDataPath);

            //激活退出处理器
            //QuitHandler.Instance.ActiveHandler();

            //显示开始界面
            //UIManager.Instance.ShowPanelAsync<BeginPanel>(UIManager.E_UILayer.Mid, (panel) =>
            //{
            //    //显示下载框，隐藏其它控件
            //    panel.UpdateDownLoadFrame(true);

            //    //显示进度文本
            //    panel.GetPanelControl<TextMeshProUGUI>("txtPro").gameObject.SetActive(false);

            //    //显示下载速度
            //    panel.GetPanelControl<TextMeshProUGUI>("txtProSpeed").gameObject.SetActive(false);

            //    //检查更新
            //    ABUpdateMgr.Instance.CheckUpdate(UpdateOverCallBack, UpdatePhaseCallBack, UpdateProCallBack);
            //});
        }

        /// <summary>
        /// 更新结束回调
        /// </summary>
        /// <param name="isOver"></param>
        public void UpdateOverCallBack(bool isOver)
        {
            //if (isHit)
            //{
            //    StartCoroutine(UpdateOver());
            //}
            //else
            //{
            //    //显示提示界面
            //    UIManager.Instance.ShowPanelAsync<TipPanel>(UIManager.E_UILayer.Bot, (tipPanel) =>
            //    {
            //        UIManager.Instance.GetPanel<BeginPanel>((beginPanel) =>
            //        {
            //            //初始化提示信息
            //            tipPanel.InitTip(beginPanel.GetNetMsg(), sureCallBack: () =>
            //            {
            //                //隐藏下载框，显示其它控件
            //                beginPanel.UpdateDownLoadFrame(false);
            //            });
            //        });
            //    });
            //}
        }

        /// <summary>
        /// 更新阶段回调
        /// </summary>
        /// <param name="phase"></param>
        public void UpdatePhaseCallBack(E_UpdatePhase phase)
        {
            //this.phase = phase;
            //UIManager.Instance.GetPanel<BeginPanel>((panel) =>
            //{       
            //    //当前阶段是下载对比文件，就开启协程，更新动态效果
            //    if (phase == ABUpdateMgr.E_UpdatePhase.DownLoadRemoteCompareFile)
            //    {
            //        StartCoroutine(UpdateLoadingText_Cor(panel));
            //    }
            //    else
            //    {
            //        //下载进度文本显示
            //        panel.GetPanelControl<TextMeshProUGUI>("txtPhase").text = GetDownLoadPhase(phase);
            //    }
            //});

            ////更新文本动态效果
            //IEnumerator UpdateLoadingText_Cor(BeginPanel panel)
            //{
            //    int index = 3;
            //    while (this.phase == ABUpdateMgr.E_UpdatePhase.DownLoadRemoteCompareFile)
            //    {
            //        string loadingStr = GetDownLoadPhase(this.phase);  //正在下载远端对比文件...
            //        panel.GetPanelControl<TextMeshProUGUI>("txtPhase").text = loadingStr.Substring(0, loadingStr.LastIndexOf('.') - index + 1);
            //        --index;
            //        if (index < 0)
            //            index = 3;
            //        yield return new WaitForSeconds(0.35f);
            //    }
            //}
        }

        /// <summary>
        /// 更新进度回调
        /// </summary>
        /// <param name="nowDownSizeThisFrame"></param>
        /// <param name="allSize"></param>
        public void UpdateProCallBack(ulong nowDownSizeThisFrame, ulong allSize)
        {
            //if (phase == ABUpdateMgr.E_UpdatePhase.DownLoadABRes)
            //{
            //    UIManager.Instance.GetPanel<BeginPanel>((panel) =>
            //    {
            //        //显示进度文本
            //        panel.GetPanelControl<TextMeshProUGUI>("txtPro").gameObject.SetActive(true);

            //        //显示下载速度
            //        panel.GetPanelControl<TextMeshProUGUI>("txtProSpeed").gameObject.SetActive(true);

            //        //累加每帧下载字节数
            //        nowDownLoadSize += nowDownSizeThisFrame;

            //        //下载进度文本显示
            //        panel.GetPanelControl<TextMeshProUGUI>("txtPro").text = TextUtility.FloatToStr((float)nowDownLoadSize / allSize * 100, 2) + "%";

            //        //下载进度条显示
            //        panel.GetPanelControl<Slider>("sliderPro").value = (float)nowDownLoadSize / allSize;

            //        //下载速度显示
            //        if (nowDownSizeThisFrame != 0)
            //        {
            //            nowNum = 0;
            //            panel.GetPanelControl<TextMeshProUGUI>("txtProSpeed").text = TextUtility.ToByteUnit(nowDownSizeThisFrame);
            //        }
            //        else
            //        {
            //            ++nowNum;
            //            if (nowNum >= maxNum)
            //            {
            //                panel.GetPanelControl<TextMeshProUGUI>("txtProSpeed").text = TextUtility.ToByteUnit(nowDownSizeThisFrame);
            //            }
            //        }
            //    });
            //}
        }

        /// <summary>
        /// 获取下载阶段
        /// </summary>
        /// <param name="phase"></param>
        /// <returns></returns>
        private string GetDownLoadPhase(E_UpdatePhase phase)
        {
            switch (phase)
            {
                case E_UpdatePhase.DownLoadRemoteCompareFile:
                    return "正在下载远端对比文件...";
                case E_UpdatePhase.GetLocalCompareFile:
                    return "获取本地对比文件中...";
                case E_UpdatePhase.DownLoadAssets:
                    return "正在下载资源...";
                case E_UpdatePhase.Finished:
                    return "更新完成";
                default:
                    LogMgr.LogError($"没有实现该枚举项：{phase}");
                    return null;
            }
        }

        /// <summary>
        /// 更新结束
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateOver()
        {
            //    BeginPanel beginPanel = null;
            //    UIManager.Instance.GetPanel<BeginPanel>((panel) =>
            //    {
            //        beginPanel = panel;
            //        panel.GetPanelControl<TextMeshProUGUI>("txtPro").text = "100%";
            //        panel.GetPanelControl<Slider>("sliderPro").value = 1;
            //    });

            //    yield return new WaitForSeconds(0.4f);

            //    //隐藏下载框，显示其它控件
            //    beginPanel.UpdateDownLoadFrame(false);

            yield return 0;
        }
    }
}
