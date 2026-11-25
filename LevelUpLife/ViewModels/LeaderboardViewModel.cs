using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LevelUpLife.Core.Services;
using LevelUpLife.Infrastructure;
using Serilog;

namespace LevelUpLife.ViewModels;

/// <summary>
/// Окно таблицы лидеров.
/// </summary>
public class LeaderboardViewModel : ViewModelBase
{
    private const int TopCount = 20;

    private readonly ILeaderboardService _leaderboard;
    private readonly IUserContext _userContext;
    private string _myRankText = string.Empty;
    public LeaderboardViewModel(ILeaderboardService leaderboard, IUserContext userContext)
    {
        _leaderboard = leaderboard;
        _userContext = userContext;

        RefreshCommand = new RelayCommand(Load);
        CloseCommand = new RelayCommand(RequestClose);
    }
    public ObservableCollection<LeaderboardEntry> Entries { get; } = new();
    public string MyRankText
    {
        get => _myRankText;
        private set => SetProperty(ref _myRankText, value);
    }
    public ICommand RefreshCommand { get; }

    public ICommand CloseCommand { get; }
    public void Load()
    {
        try
        {
            ErrorMessage = string.Empty;

            Entries.Clear();
            foreach (LeaderboardEntry entry in _leaderboard.GetTop(TopCount))
            {
                Entries.Add(entry);
            }

            int rank = _leaderboard.GetRank(_userContext.CurrentUserId);
            MyRankText = rank > 0 ? $"Ваше место: {rank}" : string.Empty;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка загрузки таблицы лидеров");
            ErrorMessage = "Не удалось загрузить таблицу лидеров.";
        }
    }
}
