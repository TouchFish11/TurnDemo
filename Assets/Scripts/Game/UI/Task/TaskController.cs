using Framework;
using Game.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MVC
{
    /// <summary>
    /// 任务界面控制器工厂
    /// </summary>
    public class TaskControllerFactory : UIControllerFactory<TaskView, TaskModel, TaskController>
    {
        public override TaskController CreateController(TaskView view, TaskModel model)
        {
            return new TaskController(view, model);
        }

        public override TaskModel CreateModel()
        {
            return new TaskModel();
        }
    }

    /// <summary>
    /// 任务界面控制器
    /// </summary>
    public class TaskController : UIController<TaskView, TaskModel>
    {
        public TaskController(TaskView view, TaskModel model) : base(view, model)
        {
        }

        protected override void OnInit()
        {

        }

        protected override void ButtonOnClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    UIManager.Instance.HideView<TaskView, TaskModel, TaskController>();
                    break;
            }
        }





    }
}
