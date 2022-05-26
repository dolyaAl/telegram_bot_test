using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Telegram.Bot;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private MyBot Bot = new MyBot();

        public Form1()
        {
            InitializeComponent();
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            if (!this.BotBGWorker.IsBusy)
            {
                this.BotBGWorker.RunWorkerAsync();
                MessageBox.Show("bot started");
            }
        }

        async private void BotBGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            try
            {
                await Bot.Bot.SetWebhookAsync("");
                Bot.DoBotUpdates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error : {ex.Message}");
            }
        }
    }
    public class MyBot
    {
        public TelegramBotClient Bot;
        private Dictionary<string, string> commands;
        private Telegram.Bot.Types.Update[] updates;
        private string BotToken;
        private string lastCommand;
        private int offset;
        public MyBot()
        {
            BotToken = "5188364517:AAG7ps-ZXwvfmMBdDj9olIWKsrHZfVh4WA4";
            Bot = new TelegramBotClient(BotToken);
            commands = new Dictionary<string, string>
            {
                {"/start", "Начало работы"},
                {"/help", "Помощь в работе"},
                {"/sum", "Пришлите числа через пробел"},
                {"/game", "game sending"}
            };
            lastCommand = "";
            offset = 0;
        }
        async public void SetWeb(string web)
        {
            await Bot.SetWebhookAsync("");
        }
        async public void DoBotUpdates()
        {
            updates = await Bot.GetUpdatesAsync(offset);
            while (true)
            {
                foreach (var update in updates)
                {
                    if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                    {
                        DoCallbackQuery(update.CallbackQuery);
                    }
                    if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
                    {
                        DoMessage(update.Message);
                    }
                    offset = update.Id + 1;
                    updates = await Bot.GetUpdatesAsync(offset);
                }
            }
        }
        async private void DoCallbackQuery(Telegram.Bot.Types.CallbackQuery query)
        {
            if (query.GameShortName == "testgame")
            {
                await Bot.AnswerCallbackQueryAsync(query.Id, null, null, "https://maxizhukov.github.io/telegram_game_front/");
            }
        }
        async private void DoMessage(Telegram.Bot.Types.Message message)
        {
            var text = message.Text;
            if (commands.ContainsKey(text))
            {
                lastCommand = text;
                await Bot.SendTextMessageAsync(message.Chat.Id, commands[text]);
            }
            if (lastCommand == "/game")
            {
                await Bot.SendGameAsync(message.Chat.Id, "testgame");
            }
        }
    }
}
