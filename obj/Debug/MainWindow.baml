   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	   F BSudokuMaker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:SudokuMaker.ModelSudokuMaker.Model 5   	   I)clr-namespace:SudokuMaker.Util.ConvertersSudokuMaker.Util.Converters 5   	      SudokuMaker.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   9System&clr-namespace:System;assembly=mscorlib     5   	   0Gameclr-namespace:SudokuMaker.Model     5   	   7c)clr-namespace:SudokuMaker.Util.Converters     5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	     � Title$  
MainWindow��5   	   $	��600��6   $	��700��6)   $	��500��65   $	��600��6D    � 	Resources 5	         DifficultyEnum S� 
MethodName S� 
ObjectType   System.Enum S� MethodParameters M� Type"  SudokuMaker.Model.Difficulty9  3SudokuMaker.Util.Converters.InverseBooleanConverter  NotBoolConverter��%�   &	        &	 `     S� 5
   
   $ 	GetValues��64   "  6K    5      M� 5      "  6   62   5        5   
   6?   5      .��5      ��   BackgroundBrush#��[ 6   $��3q�6<     MainBackgroundStyle#��[ 6P   ._�5   
   �  � RowDefinitions 5      �� 5      $
��0.1* �6    �� 5      $
��0.1* �6    �� 5      $
��0.8* �6    �� 5      $
��0.1* �6     � ColumnDefinitions 5      �� 5      $
��0.7* �6#   �� 5      $
��0.3* �6#   .+�5       o� $��25��6   $��2��6   $	��Top=�63     	MenuStyle#��[ 6K   #��[ 6n   .@�5!      m� $
��Game��6   .@�5"      m� $
��Load��6    m� Command 6-   �� *LoadGameCommand+m� 5#      $
��Save��6   {�6-   �� *Game.IsStarted+ 6R   �� *SaveGameCommand+m� 5$      $��Rules��6   m� 5%      $��
Statistics��6    63   �� *ShowStatsCommand+m� 5&      $
��Exit��6    6-   �� *ExitCommand+5'      m� 5(      $��About��6   .@�5)      m� $��About Sudoku��6   m� 5*      $��About author��6   5+      5,      �� 5.      $��1��6   $��2��6'   $��Stretch=�64   $��	10,0,10,0q�6R   $	��400��6e   ��5/      �� 50      $��Georgia)�6    $��16(�65   $��Center=�6C   Game Options6n   .��52      �� .+�53      �� $��Center=�6    $��16(�6>   $��Georgia)�6L   Select level:6p   �� 54      -   5        	 ��Name$	 
ComboLevel��54      $��Georgia)�63   $��14(�6H   $��5q�6�   ��6V   �� 
 �� Source#
 [   �� SelectedItem 55      ��  �� Path$ Game.Difficulty�{�6M   �� $ Game.IsStarted� �� 	Converter# [ �� 56      -   5         ��Name$ StartButton��56        ButtonGameOptionsStyle#��[ 62   $��New Game��6�   ��6b   �� *NewGameCommand+{�57      �� $ Game.IsStarted�# [ 58      �� 59      -   5        $ 
StopButton��59      #��[ 61   $
��Stop��5:   C   ��59   a   �� *StopGameCommand+{�5:      �� $ Game.IsStarted�5;      �� 5<      -   5        $ CheckButton��5<      #��[ 62   $��Check��5=   C   ��5<   b   �� *CheckCommand+{�5=      �� $ Game.IsStarted�5>      5?      5@      �� 5C       �� TextAlignment$ Center=�6   $��30(�6/   $��Comic Sans MS)�6=   $��1��5D      $��Center=�6%   $	��Top=�6B   ���#3b5998Sudoku6w   �� 5E      -   5         ��Name$ CellsCollection��5E      $��5,0,0,0q�6V   $��2��6g   $	��400��6t   $	��400��6�   $	��400��6�   $	��400��6�   $��Center=�6�   ��64   �� *	UserCells+��5F      �� 5G      .R�5H      A� 6(   5I      ��5L      �� 5M      .R�5N      ��  ��Name$ 
CellBorder��6!   
���   ���65   �� *BorderThickness+._�5O      ��  ��Name$ CellBox��5P   *    �� TextAlignment$ Center=�5Q   *   $��Stretch=�5R   *    �� 
CaretBrush
 �����$��27(�5U   *   $�� ��5V   *   $��Comic Sans MS)�5W   *    �� 	MaxLength$ 1��5X   *   $	��0.5q�5Y   *    J� Opacity$ 0Q�5Z   *   $��0q�5[   *   ��5S   *   �� *Color+ �� UpdateSourceTrigger$ PropertyChanged=�.��5[   >   .  (System.Windows.Interactivity.Interaction   Triggers 5\   "   /  )System.Windows.Interactivity.EventTrigger  5]   &      	EventName$ PreviewKeyUp��65   .  (System.Windows.Interactivity.TriggerBase   Actions. 5^   *   6  0System.Windows.Interactivity.InvokeCommandAction     Command 6@   ��  �� ElementName$ CellsCollection��$# DataContext.CheckCellsCommand�   CommandParameter 5_   V   �� *
Position+5`   '   .��5a   :   5b      5c       �� Triggers 5d      �� 5e       �� Value$
 True��6�     �� Binding  6*   �� $ CellsCollection��$  DataContext.Game.IsStarted�$ PropertyChanged=�.a�5f   "   �� ! �� 
TargetName$! CellBox��69   " �� Value" 6N   �� *	StrNumber+$ PropertyChanged=�# �� Property# � ��$ N� EnterActions$ 5g   "   �� 5h   &   .��5i   *   �� .��5j   .   W� % g� Duration$% 0:0:3D�6>   & W� From$& 0X�6O   	' W� To$' 1X�6X   ( �� TargetProperty$( Opacity�6_   ) �� 
TargetName$) CellBox��6�   5k   +   5l   '   5n      P� 5o      * P� 
SourceName$* CellBox��6;   + P� Property+ �@J�	IsFocused, P� Value, . . �5p   "   �� $! CellBox��6?   # � ��
" �����6T   �� 5q   "   $! 
CellBorder��6D   " 6\   �� *BorderThickness+# � ��5r      5t      ��5w      �� 5x      .�5y      �� # � ��" 68   �� *
Position.X+�� 5z      # � ��" 65   �� *
Position.Y+5{      5}      �� 5      -   5        - ��Name$- footerPanel��5      $��25��6.   $��3��6:   $��0��6G   $��2��6W   $��Bottom=�6k   . �� Orientation$. 
Horizontal=�6�   .+�5�      �� $��Georgia)�6   $��13(�61   $��Center=�6?   $��15,0,0,0q�6Z   $��Time spent: ��6l   6�   �� 5�      $��Center=�6   $��Georgia)�67   $��13(�6L   ��5�      `� 5�      / �� StringFormat$/ {0:00}:{1:00}��6'   .1�5�      �� $ Game.SpendTime.Minutes�6&   �� 5�      $ Game.SpendTime.Seconds�6&   5�      5�      5�      5�      5�       5�        5�   
   $ Closing��6   . 5�         6%   �� *ExitCommand+5�      5�      