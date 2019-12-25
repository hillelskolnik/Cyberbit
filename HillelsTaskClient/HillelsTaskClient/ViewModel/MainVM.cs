using HillelsTaskClient.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HillelsTaskClient.ViewModel
{
    public class MainVM : NotifyChange
    {
        private const string baseUrl = @"http://localhost:62525/";
        public ICommand ButtonCommand { get; set; }
        public MainVM()
        {
            ButtonCommand = new RelayCommand(new Action<object>(Button_Click), new Predicate<object>(Button_CanExecute));
        }

        public bool Button_CanExecute(object sender)
        {
            return true;
        }

        public ObservableCollection<TaskDue> Done { get; private set; } = new ObservableCollection<TaskDue>();
        public ObservableCollection<TaskDue> Late { get; private set; } = new ObservableCollection<TaskDue>();
        public ObservableCollection<TaskDue> OnTime { get; private set; } = new ObservableCollection<TaskDue>();

        private bool isLogin = false;

        public bool IsLogIn
        {
            get { return isLogin; }
            set 
            {
                if (isLogin == value)
                {
                    return;
                }
                isLogin = value;
                Notify("IsLogIn");
            }
        }

        private TaskDue lateSelected;

        public TaskDue LateSelected
        {
            get { return lateSelected; }
            set
            {
                if (lateSelected == value || value == null)
                {
                    return;
                }
                lateSelected = value;
                SelectedTaskId = lateSelected.Id;
                SelectedTaskTitle = lateSelected.TaskName;
            }
        }

        private TaskDue onTimeSelected;

        public TaskDue OnTimeSelected
        {
            get { return onTimeSelected; }
            set 
            {
                if (onTimeSelected == value || value == null)
                {
                    return;
                }
                onTimeSelected = value;
                SelectedTaskId = onTimeSelected.Id;
                SelectedTaskTitle = onTimeSelected.TaskName;
            }
        }

        public int SelectedTaskId { get; set; } = 0;
        private string selectedTasktitle;

        public string SelectedTaskTitle
        {
            get { return selectedTasktitle; }
            set 
            { 
                selectedTasktitle = value;
                Notify("SelectedTaskTitle");
            }
        }



        public DateTime DueDate { get; set; } = DateTime.Now;

        public string UserName { get; set; }
        public int Token { get; set; } = -1;

        public String TaskTitle { get; set; }

        private async Task AddTask()
        {
            var url = $"{baseUrl}InsertTasks?userToken={Token}&title={TaskTitle}&dueDate={DueDate}";
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync(url, null);
                await FillLsts();
            }
        }

        private async Task LogIn()
        {
            var url = $"{baseUrl}UserToken?name={UserName}";
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(url);
                int token;
                if (int.TryParse(result, out token))
                {
                    Token = token;
                    IsLogIn = true;

                    await FillLsts();
                }
            }
        }

        private async Task SetAsDone()
        {
            var url = $"{baseUrl}SetTaskDone?id={SelectedTaskId}";
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync(url, null);
                await FillLsts();
            }

        }

        private async Task FillLsts()
        {
            var url = $"{baseUrl}Tasks?userToken={Token}";
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(url);
                var list = JsonConvert.DeserializeObject<List<TaskDue>>(result);
                var done = list.Where(x => x.Done).ToList();
                DateTime dtNow = DateTime.Now;
                var late = list.Where(x => !x.Done && x.DueDate > dtNow).ToList();
                var onTime = list.Where(x => !x.Done && x.DueDate <= dtNow).ToList();

                Done.Clear();
                Late.Clear();
                OnTime.Clear();
                done.ForEach(item =>
                {
                    Done.Add(item);
                });

                late.ForEach(item =>
                {
                    Late.Add(item);
                });

                onTime.ForEach(item =>
                {
                    OnTime.Add(item);
                });
            }
        }
        public void Button_Click(object sender)
        {
            MethodInfo methodInfo = GetType().GetMethod(sender.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo?.Invoke(this, null);
        }
    }
}
