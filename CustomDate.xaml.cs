using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace nofun.control
{
    /// <summary>
    /// CustomDate.xaml 的交互逻辑
    /// </summary>
    public partial class CustomDate : UserControl, INotifyPropertyChanged
    {
        public CustomDate()
        {
            InitializeComponent();
            this.Loaded += (s, e) => initCalendarData();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;



        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public enum CalendarViewMode
        {
            YearView,
            MonthView,
            DayView
        }


        private CalendarViewMode _calendarViewMode = CalendarViewMode.DayView;

        public CalendarViewMode calendarViewMode
        {
            get => _calendarViewMode;
            set
            {
                if (_calendarViewMode != value)
                {
                    _calendarViewMode = value;
                    OnCalendarViewModeChanged(_calendarViewMode);
                    OnPropertyChanged();
                }
            }
        }

        private int selectedDay;

        public event EventHandler<string> DateSelected;



        public void ResetData()
        {
            if (selectedDay == 0)
            {
                year = DateTime.Today.Year;
                month = DateTime.Today.Month;
            }
            calendarViewMode = CalendarViewMode.DayView;
        }

        private void OnCalendarViewModeChanged(CalendarViewMode viewMode)
        {
            switch (viewMode)
            {
                case CalendarViewMode.DayView:
                    updateDayView();
                    break;
                case CalendarViewMode.MonthView:
                    updateMonthView();
                    break;
                case CalendarViewMode.YearView:
                    updateYearView();
                    break;
            }
        }

        private void updateDayView()
        {
            CurrentYearMonth = year + "年" + month + "月";
            addCurrentMonthOfWeekToList(year, month);
            addWeekTextList();
            clearOtherView();
        }


        private void updateMonthView()
        {
            CurrentYearMonth = year.ToString();
            addMonthView(year);
            clearOtherView();

        }

        private void updateYearView()
        {
            year = year / 10 * 10;
            CurrentYearMonth = year + "-" + (year + 9);
            addYearView(year - 1, year + 10);
            clearOtherView();
        }


        private void addYearView(int startYear, int endYear)
        {
            Brush? yearSelectedBrush = new BrushConverter().ConvertFrom("#0F2A39") as Brush;
            Brush? yearBorderBrush = new BrushConverter().ConvertFrom("#8FE9EA") as Brush;
            Brush? yearBrush = new BrushConverter().ConvertFrom("#1A4151") as Brush;
            YearList.Children.Clear();
            int currentYear = SelectedDate == null ? DateTime.Today.Year : DateTime.Parse(SelectedDate).Year;
            for (int y = startYear; y <= endYear; y++)
            {
                Border border = new Border
                {
                    Background = Brushes.Transparent,
                    CornerRadius = new CornerRadius(5)
                };
                border.Tag = y;
                if (currentYear == y)
                {
                    border.Background = yearSelectedBrush;
                    border.BorderBrush = yearBorderBrush;
                    border.BorderThickness = new Thickness(1);
                    border.Margin = new Thickness(-1);
                }
                border.MouseEnter += (s, e) =>
                {
                    if (currentYear == (int)border.Tag) border.Opacity = 0.7;
                    else border.Background = yearBrush;
                };
                border.MouseLeave += (s, e) =>
                {
                    if (currentYear == (int)border.Tag) border.Opacity = 1;
                    else border.Background = Brushes.Transparent;
                };
                border.MouseDown += (s, e) =>
                {
                    TextBlock textBlock = (TextBlock)border.Child;
                    if (!textBlock.Text.Equals("/"))
                    {
                        year = int.Parse(textBlock.Text);
                        calendarViewMode = CalendarViewMode.MonthView;
                    }
                    updateLeftArrowOpacity();
                };
                TextBlock textBlock = new TextBlock
                {
                    Text = y > 0 ? y.ToString() : "/",
                    FontSize = 13,
                    Foreground = Brushes.White
                };
                if (y == startYear || y == endYear)
                {
                    textBlock.Opacity = 0.5;
                }
                border.Child = textBlock;
                YearList.Children.Add(border);
            }
        }







        private void clearOtherView()
        {
            if (calendarViewMode == CalendarViewMode.DayView)
            {
                DayList.Visibility = Visibility.Visible;
                YearList.Visibility = Visibility.Collapsed;
                MonthList.Visibility = Visibility.Collapsed;
                WeekList.Visibility = Visibility.Visible;
            }
            else if (calendarViewMode == CalendarViewMode.MonthView)
            {
                DayList.Visibility = Visibility.Collapsed;
                YearList.Visibility = Visibility.Collapsed;
                MonthList.Visibility = Visibility.Visible;
                WeekList.Visibility = Visibility.Collapsed;
            }
            else
            {
                DayList.Visibility = Visibility.Collapsed;
                YearList.Visibility = Visibility.Visible;
                MonthList.Visibility = Visibility.Collapsed;
                WeekList.Visibility = Visibility.Collapsed;
            }
        }




        private void addMonthView(int year)
        {

            MonthList.Children.Clear();
            Brush? monthBrush = new BrushConverter().ConvertFrom("#1A4151") as Brush;
            Brush? monthSelectedBrush = new BrushConverter().ConvertFrom("#0F2A39") as Brush;
            Brush? monthBorderBrush = new BrushConverter().ConvertFrom("#8FE9EA") as Brush;
            List<string> monthNameList = ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"];

            int currentYear = SelectedDate == null ? DateTime.Today.Year : DateTime.Parse(SelectedDate).Year;
            int currentMonth = SelectedDate == null ? DateTime.Today.Month : DateTime.Parse(SelectedDate).Month;
            for (int i = 0; i < 12; i++)
            {
                Border border = new Border
                {
                    Background = Brushes.Transparent,
                    CornerRadius = new CornerRadius(5),
                };
                border.Tag = i + 1;
                if (currentYear == year && currentMonth == i + 1)
                {
                    border.Background = monthSelectedBrush;
                    border.BorderBrush = monthBorderBrush;
                    border.BorderThickness = new Thickness(1);
                    border.Margin = new Thickness(-1);
                }
                border.MouseEnter += (s, e) =>
                {
                    if (currentYear == year && currentMonth == (int)border.Tag) border.Opacity = 0.7;
                    else border.Background = monthBrush;
                };
                border.MouseLeave += (s, e) =>
                {
                    if (currentYear == year && currentMonth == (int)border.Tag) border.Opacity = 1;
                    else border.Background = Brushes.Transparent;
                };
                border.MouseDown += (s, e) =>
                {
                    TextBlock textBlock = (TextBlock)border.Child;
                    string monthText = textBlock.Text;
                    month = int.Parse(monthText[..^1]);
                    calendarViewMode = CalendarViewMode.DayView;
                    updateLeftArrowOpacity();
                };

                TextBlock textBlock = new TextBlock
                {
                    Foreground = Brushes.White,
                    FontSize = 13,
                    Text = monthNameList[i]
                };

                border.Child = textBlock;
                MonthList.Children.Add(border);
            }
        }






        private void initCalendarData()
        {
            CurrentYearMonth = year + "年" + month + "月";
            addCurrentMonthOfWeekToList(year, month);
            addWeekTextList();
        }

        private void CurrentYearMonth_Click(object sender, RoutedEventArgs e)
        {
            if (calendarViewMode == CalendarViewMode.MonthView) calendarViewMode = CalendarViewMode.YearView;
            if (calendarViewMode == CalendarViewMode.DayView) calendarViewMode = CalendarViewMode.MonthView;
            updateLeftArrowOpacity();
        }


        private int year = DateTime.Today.Year;
        private int month = DateTime.Today.Month;


        private string _currentYearMonth;
        public string CurrentYearMonth
        {
            get => _currentYearMonth;
            set
            {
                if (_currentYearMonth != value)
                {
                    _currentYearMonth = value;
                    OnPropertyChanged();
                }
            }
        }


        private string _selectedDate;
        public string SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                }
            }
        }


        private void updateLeftArrowOpacity()
        {
            if (calendarViewMode == CalendarViewMode.DayView)
            {
                if (year <= 1 && month <= 1) LeftPrevious.Opacity = 0.5;
                else LeftPrevious.Opacity = 1;
            }
            else if (calendarViewMode == CalendarViewMode.MonthView)
            {
                if (year <= 1) LeftPrevious.Opacity = 0.5;
                else LeftPrevious.Opacity = 1;
            }
            else
            {
                if (year <= 0) LeftPrevious.Opacity = 0.5;
                else LeftPrevious.Opacity = 1;
            }
        }





        List<KeyValuePair<int, int>> monthWeekList = new List<KeyValuePair<int, int>>();


        private int oneCountStatus = -1;

        void addCurrentMonthOfWeekToList(int year, int month)
        {
            monthWeekList.Clear();
            int daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                int weekValue = getWeekDayOfWeek(year, month, day);
                monthWeekList.Add(new KeyValuePair<int, int>(day, weekValue));
            }
            addPreivousMonthOfWeekToList(year, month, daysInMonth);
            addNextMonthOfWeekToList(year, month, daysInMonth);

            List<int> keys = monthWeekList.Select(kvp => kvp.Key).ToList();
            DayList.Children.Clear();
            int oneCount = 0;
            foreach (int key in keys)
            {
                if (key == 1) oneCount++;
                TextBlock text = new TextBlock
                {
                    Text = key.ToString(),
                    Foreground = Brushes.White,
                    FontSize = 13,
                    Opacity = oneCount == 1 ? 1 : 0.5
                };
                bool isToday = judgeIsToday(year, month, key) && oneCount == 1;
                Brush? todayBrush = new BrushConverter().ConvertFrom("#00CBB3") as Brush;
                Brush? mouseDownBrush = new BrushConverter().ConvertFrom("#2597AD") as Brush;
                Brush? mouseHoverBrush = new BrushConverter().ConvertFrom("#1A4151") as Brush;

                Border border = new Border
                {
                    Background = isToday ? todayBrush : Brushes.Transparent,
                    CornerRadius = new CornerRadius(3)
                };
                border.Tag = oneCount;
                if (!isToday)
                {
                    if (oneCountStatus == (int)border.Tag && selectedDay == key)
                    {
                        border.Background = mouseDownBrush;
                        border.MouseEnter += (s, e) =>
                        {
                            border.Opacity = 0.7;
                        };
                        border.MouseDown += (s, e) =>
                        {
                            TextBlock textBlock = (TextBlock)border.Child;
                            int day = int.Parse(textBlock.Text);
                            selectedDay = day;
                            oneCountStatus = (int)border.Tag;
                            SelectedDate = $"{year}/{month:D2}/{day:D2}";
                            DateSelected?.Invoke(this, SelectedDate);
                            updateLeftArrowOpacity();
                        };
                        border.MouseLeave += (s, e) =>
                        {
                            border.Opacity = 1;
                        };
                    }
                    else
                    {
                        border.MouseEnter += (s, e) =>
                        {
                            border.Background = mouseHoverBrush;
                        };
                        border.MouseDown += (s, e) =>
                        {
                            border.Background = mouseDownBrush;
                            TextBlock textBlock = (TextBlock)border.Child;
                            int day = int.Parse(textBlock.Text);
                            selectedDay = day;
                            oneCountStatus = (int)border.Tag;
                            SelectedDate = $"{year}/{month:D2}/{day:D2}";
                            DateSelected?.Invoke(this, SelectedDate);
                            updateLeftArrowOpacity();
                        };

                        border.MouseLeave += (s, e) =>
                        {
                            border.Background = Brushes.Transparent;
                        };
                    }
                }
                else
                {
                    border.MouseEnter += (s, e) =>
                    {
                        border.Opacity = 0.7;
                    };
                    border.MouseDown += (s, e) =>
                    {
                        TextBlock textBlock = (TextBlock)border.Child;
                        int day = int.Parse(textBlock.Text);
                        selectedDay = day;
                        oneCountStatus = (int)border.Tag;
                        SelectedDate = $"{year}/{month:D2}/{day:D2}";
                        DateSelected?.Invoke(this, SelectedDate);
                        updateLeftArrowOpacity();
                    };
                    border.MouseLeave += (s, e) =>
                    {
                        border.Opacity = 1;
                    };
                }

                border.Child = text;
                DayList.Children.Add(border);
            }

        }

        void addWeekTextList()
        {
            WeekList.Children.Clear();
            List<string> weekNames = ["一", "二", "三", "四", "五", "六", "日"];
            for (int i = 0; i < 7; i++)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = weekNames[i]
                };
                WeekList.Children.Add(textBlock);
            }

        }

        bool judgeIsToday(int year, int month, int day)
        {
            DateTime today = DateTime.Today;
            return year == today.Year && month == today.Month && day == today.Day;
        }


        void addPreivousMonthOfWeekToList(int year, int month, int daysInMonth)
        {
            int firstDayWeek = getWeekDayOfWeek(year, month, 1);
            int needDays = firstDayWeek == 0 ? 6 : firstDayWeek - 1;

            if (year <= 1 && month <= 1) return;
            month--;
            if (month == 0)
            {
                year--;
                month = 12;
            }
            int day = DateTime.DaysInMonth(year, month);


            for (int i = 0; i < needDays; i++)
            {
                int week = getWeekDayOfWeek(year, month, day);
                monthWeekList.Insert(0, new KeyValuePair<int, int>(day, week));
                day--;
            }

        }


        void addNextMonthOfWeekToList(int year, int month, int daysInMonth)
        {
            month++;
            if (month == 13)
                if (month == 13)
                {
                    year++;
                    month = 1;
                }
            int day = 1;
            while (monthWeekList.Count() < 42)
            {
                int week = getWeekDayOfWeek(year, month, day);
                monthWeekList.Add(new KeyValuePair<int, int>(day, week));
                day++;
            }

        }


        private void LeftArrow_Click(object sender, MouseButtonEventArgs e)
        {
            if (calendarViewMode == CalendarViewMode.DayView)
            {
                if (year <= 1 && month <= 1) return;
                month--;
                if (month == 0)
                {
                    month = 12;
                    year--;
                }
                CurrentYearMonth = year + "年" + month + "月";
                addCurrentMonthOfWeekToList(year, month);
            }
            else if (calendarViewMode == CalendarViewMode.MonthView)
            {
                if (year <= 1) return;
                year--;
                CurrentYearMonth = year.ToString();
                addMonthView(year);
            }
            else
            {
                if (year <= 0) return;
                year = year / 10 * 10 - 10;
                CurrentYearMonth = year + "-" + (year + 9);
                addYearView(year - 1, year + 10);
            }
            updateLeftArrowOpacity();
        }

        private void RightArrow_Click(object sender, MouseButtonEventArgs e)
        {
            if (calendarViewMode == CalendarViewMode.DayView)
            {
                month++;
                if (month == 13)
                {
                    month = 1;
                    year++;
                }
                CurrentYearMonth = year + "年" + month + "月";
                addCurrentMonthOfWeekToList(year, month);
            }
            else if (calendarViewMode == CalendarViewMode.MonthView)
            {
                year++;
                CurrentYearMonth = year.ToString();
                addMonthView(year);
            }
            else
            {
                year = year / 10 * 10 + 10;
                CurrentYearMonth = year + "-" + (year + 9);
                addYearView(year - 1, year + 10);
            }
            updateLeftArrowOpacity();
        }



        int getWeekDayOfWeek(int year, int month, int day)
        {
            DateTime currentDay = new DateTime(year, month, day);
            return (int)currentDay.DayOfWeek;
        }



    }





}
