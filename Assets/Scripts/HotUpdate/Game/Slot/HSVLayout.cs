namespace HotUpdate.Game.Slot
{
    /// <summary>
    /// 水平滚动视图布局组件
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <typeparam name="K">格子脚本</typeparam>
    public class HSVLayout<T, K> where K : IGridBase<T>
    {
        // //子对象滚动视图
        // private ScrollRect _horizontalScrollRect;
        // //滚动视图Content
        // private RectTransform _content;
        // //存储格子预设体
        // private List<GameObject> _gridList = new List<GameObject>();
        // //水平间隔
        // private float xSpace;
        // //格子宽
        // private float gridWidth;
        //
        // /// <summary>
        // /// 初始化水平滚动视图布局组件
        // /// </summary>
        // /// <param name="scrollRect">显示格子的滚动视图</param>
        // /// <param name="gridWidth">格子宽度</param>
        // /// <param name="xSpace">格子水平间隔</param>
        // /// <returns>初始化后的水平滚动视图布局组件对象</returns>
        // public HSVLayout<T, K> InitHSV(ScrollRect scrollRect, float gridWidth, float xSpace)
        // {
        //     this._horizontalScrollRect = scrollRect;
        //     this._content = scrollRect.content;
        //     this.gridWidth = gridWidth;
        //     this.xSpace = xSpace;
        //     return this;
        // }
        //
        // /// <summary>
        // /// 更新视图显示
        // /// </summary>
        // /// <param name="dataList">数据列表</param>
        // public void UpdateView(List<T> dataList, GameObject gridObj)
        // {
        //     //清除上次创建的对象
        //     for (int i = 0; i < _gridList.Count; i++)
        //     {
        //         GameObject.Destroy(_gridList[i].gameObject);
        //     }
        //     _gridList.Clear();
        //
        //     //count小于等于sv的宽
        //     if (dataList.Count * (xSpace + gridWidth) <= (_horizontalScrollRect.transform as RectTransform).sizeDelta.x)
        //     {
        //         //禁用滚动
        //         _horizontalScrollRect.horizontal = false;
        //         //居中显示
        //         //获取中间x位置
        //         float middlePosX = _content.sizeDelta.x / 2;
        //         //数量为奇数
        //         if (dataList.Count % 2 != 0)
        //         {
        //             //获取放置在中间位置的格子索引
        //             int middleIndex = dataList.Count / 2;
        //             for (int i = 0; i < dataList.Count; i++)
        //             {
        //                 //实例化格子对象
        //                 GameObject gridInstance = GameObject.Instantiate(gridObj, _content, false);
        //
        //                 //格子索引小于放置在中间格子的索引，放置在中间格子的左边
        //                 if (i < middleIndex)
        //                 {
        //                     //(middlePosX - (gridWidth + xSpace) / 2)：中间格子中部水平位置
        //                     //(middleIndex - i)：偏移单位，索引从0开始，偏移单位逐渐减少
        //                     //(gridWidth + xSpace)：偏移单位为（格子宽度+水平间隙）
        //                     float pos = (middlePosX - (gridWidth + xSpace) / 2) - (middleIndex - i) * (gridWidth + xSpace);
        //                     //设置中间格子左边的位置
        //                     (gridInstance.transform as RectTransform).anchoredPosition = new Vector3(pos, 0);
        //                 }
        //                 //格子索引大于放置在中间格子的索引，放置在中间格子的右边
        //                 else if(i > middleIndex)
        //                 {
        //                     //(middlePosX - (gridWidth + xSpace) / 2)：中间格子中部水平位置
        //                     //(i - middleIndex)：偏移单位，索引从大于中间格子索引开始，偏移单位逐渐增减
        //                     //(gridWidth + xSpace)：偏移单位为（格子宽度+水平间隙）
        //                     float pos = (middlePosX - (gridWidth + xSpace) / 2) + (i - middleIndex) * (gridWidth + xSpace);
        //                     //设置中间格子右边的位置
        //                     (gridInstance.transform as RectTransform).anchoredPosition = new Vector3(pos, 0);
        //                 }
        //                 //格子索引等于放置在中间格子的索引，该索引格子就是中间格子
        //                 else
        //                 {
        //                     //设置中间格子的位置
        //                     (gridInstance.transform as RectTransform).anchoredPosition = new Vector3(middlePosX - (gridWidth + xSpace) / 2, 0);
        //                 }
        //
        //                 //获取格子脚本
        //                 K grid = gridInstance.GetComponent<K>();
        //                 //初始化格子
        //                 grid.InitGrid(dataList[i], _iconProvider);
        //                 //存储格子
        //                 _gridList.Add(gridInstance);
        //             }
        //         }
        //         //数量为偶数
        //         else
        //         {
        //             for (int i = 0; i < dataList.Count; i++)
        //             {
        //                 //实例化格子
        //                 GameObject gridInstance = GameObject.Instantiate(gridObj, _content, false);
        //                 //设置位置
        //                 //dataList.Count / 2获取左边放置的格子数量
        //                 if (i < dataList.Count / 2)
        //                 {
        //                     //(gridWidth + xSpace)：偏移单位为（格子宽度+水平间隙）
        //                     //((dataList.Count / 2 - i)：偏移单位，索引从0开始，偏移单位逐渐减少
        //                     //middlePosX - ...：从中部位置向左偏移多少个格子
        //                     (gridInstance.transform as RectTransform).anchoredPosition = new Vector3(middlePosX - ((dataList.Count / 2 - i) * (gridWidth + xSpace)), 0);
        //                 }
        //                 //右边放置的格子数量等于左边放置格子的数量
        //                 else
        //                 {
        //                     //(gridWidth + xSpace)：偏移单位为（格子宽度+水平间隙）
        //                     //((i + dataList.Count / 2)：偏移单位，放置在右边格子的索引偏移单位逐渐增加
        //                     //middlePosX + ...：从中部位置向右偏移多少个格子
        //                     (gridInstance.transform as RectTransform).anchoredPosition = new Vector3(middlePosX + ((i - dataList.Count / 2) * (gridWidth + xSpace)), 0);
        //                 }
        //
        //                 //获取格子脚本
        //                 K grid = gridInstance.GetComponent<K>();
        //                 //初始化和格子
        //                 grid.InitGrid(dataList[i], TODO);
        //                 //存储格子预设体
        //                 _gridList.Add(gridInstance);
        //             }
        //         }
        //     }
        //     //count大于sv的宽
        //     else
        //     {
        //         //启用滚动
        //         _horizontalScrollRect.horizontal = true;
        //         //拓展conten的宽
        //         _content.sizeDelta = new Vector2(dataList.Count * (xSpace + gridWidth), _content.sizeDelta.y);
        //         //从左开始显示即可
        //         for (int i = 0; i < dataList.Count; i++)
        //         {
        //             //实例化格子对象
        //             GameObject gridInstance = GameObject.Instantiate(gridObj, _content, false);
        //             //设置格子位置
        //             (gridInstance.transform as RectTransform).anchoredPosition = new Vector2(i * (gridWidth + xSpace), 0);
        //             //获取格子脚本
        //             K grid = gridInstance.GetComponent<K>();
        //             //初始化格子
        //             grid.InitGrid(dataList[i], TODO);
        //             //存储格子
        //             _gridList.Add(gridInstance);
        //         }
        //     }
        // }
    }
}
