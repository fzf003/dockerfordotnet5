using BlazorHostApp.Service;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BlazorHostApp.ViewModel
{
    public class ReactiveMessage:ReactiveObject,IDisposable
    {
        readonly CompositeDisposable disposables;
        public ReactiveMessage(HttpService httpService)
        {
            this.disposables = new CompositeDisposable();

            disposables.Add(Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                      .Select(p => Observable.FromAsync(() => httpService.GetMessages()))
                      .Concat()
                      .Retry()
                      .Do(p =>
                      {

                      }, ex => Error(ex))
                      .Subscribe(p =>
                      {
                          this.Message = p;
                      }, onError: (ex) =>
                      {
                          ErrorMessage = ex.Message;
                      }));

            disposables.Add(this.WhenAnyValue(p => p.Message).Subscribe(Writer, onError: (ex) =>
            {
                ErrorMessage = ex.Message;
            }));
        }

        static void Writer(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

         void Error(Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }

        string _message = string.Empty;
        public string Message
        {
            get => this._message;
            set=>this.RaiseAndSetIfChanged(ref _message, value);
        }

        string _errormessage = string.Empty;
        public string ErrorMessage
        {
            get => this._errormessage;
            set => this.RaiseAndSetIfChanged(ref _errormessage, value);

        }
    }
}
