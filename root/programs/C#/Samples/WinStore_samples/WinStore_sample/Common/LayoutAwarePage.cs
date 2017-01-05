using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WinStore_sample.Common
{
    /// <summary>
    /// Page を一般的な方法で実装すると、重要かつ便利な機能をいくつか使用できます:
    /// <list type="bullet">
    /// <item>
    /// <description>アプリケーションのビューステートと表示状態のマップ</description>
    /// </item>
    /// <item>
    /// <description>GoBack、GoForward、および GoHome イベント ハンドラー</description>
    /// </item>
    /// <item>
    /// <description>ナビゲーション用のマウスおよびキーボードのショートカット</description>
    /// </item>
    /// <item>
    /// <description>ナビゲーションの状態管理およびプロセス継続時間管理</description>
    /// </item>
    /// <item>
    /// <description>既定のビュー モデル</description>
    /// </item>
    /// </list>
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class LayoutAwarePage : Page
    {
        /// <summary>
        /// <see cref="DefaultViewModel"/> 依存関係プロパティを識別します。
        /// </summary>
        public static readonly DependencyProperty DefaultViewModelProperty =
            DependencyProperty.Register("DefaultViewModel", typeof(IObservableMap<String, Object>),
            typeof(LayoutAwarePage), null);

        private List<Control> _layoutAwareControls;

        /// <summary>
        /// <see cref="LayoutAwarePage"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public LayoutAwarePage()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

            // 空の既定のビュー モデルを作成します
            this.DefaultViewModel = new ObservableDictionary<String, Object>();

            // このページがビジュアル ツリーの一部である場合、次の 2 つの変更を行います:
            // 1) アプリケーションのビューステートをページの表示状態にマップする
            // 2) キーボードおよびマウスのナビゲーション要求を処理する
            this.Loaded += (sender, e) =>
            {
                this.StartLayoutUpdates(sender, e);

                // キーボードおよびマウスのナビゲーションは、ウィンドウ全体を使用する場合のみ適用されます
                if (this.ActualHeight == Window.Current.Bounds.Height &&
                    this.ActualWidth == Window.Current.Bounds.Width)
                {
                    // ウィンドウで直接待機するため、フォーカスは不要です
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed +=
                        this.CoreWindow_PointerPressed;
                }
            };

            // ページが表示されない場合、同じ変更を元に戻します
            this.Unloaded += (sender, e) =>
            {
                this.StopLayoutUpdates(sender, e);
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -=
                    this.CoreWindow_PointerPressed;
            };
        }

        /// <summary>
        /// <see cref="IObservableMap&lt;String, Object&gt;"/> の実装です。これは、
        /// 単純なビュー モデルとして使用されるように設計されています。
        /// </summary>
        protected IObservableMap<String, Object> DefaultViewModel
        {
            get
            {
                return this.GetValue(DefaultViewModelProperty) as IObservableMap<String, Object>;
            }

            set
            {
                this.SetValue(DefaultViewModelProperty, value);
            }
        }

        #region ナビゲーション サポート

        /// <summary>
        /// イベント ハンドラーとして呼び出され、ページの関連付けられた <see cref="Frame"/> で前に戻ります。
        /// ナビゲーション スタックの上部に到達するまで戻ります。
        /// </summary>
        /// <param name="sender">イベントをトリガーしたインスタンス。</param>
        /// <param name="e">イベントが発生する条件を説明するイベント データ。</param>
        protected virtual void GoHome(object sender, RoutedEventArgs e)
        {
            // ナビゲーション フレームを使用して最上位のページに戻ります
            if (this.Frame != null)
            {
                while (this.Frame.CanGoBack) this.Frame.GoBack();
            }
        }

        /// <summary>
        /// イベント ハンドラーとして呼び出され、このページの <see cref="Frame"/> に関連付けられた
        /// ナビゲーション スタックで前に戻ります。
        /// </summary>
        /// <param name="sender">イベントをトリガーしたインスタンス。</param>
        /// <param name="e">イベントが発生する条件を説明する
        /// イベント データ。</param>
        protected virtual void GoBack(object sender, RoutedEventArgs e)
        {
            // ナビゲーション フレームを使用して前のページに戻ります
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }

        /// <summary>
        /// イベント ハンドラーとして呼び出され、このページの <see cref="Frame"/> に関連付けられた
        /// ナビゲーション スタックで前に戻ります。
        /// </summary>
        /// <param name="sender">イベントをトリガーしたインスタンス。</param>
        /// <param name="e">イベントが発生する条件を説明する
        /// イベント データ。</param>
        protected virtual void GoForward(object sender, RoutedEventArgs e)
        {
            // ナビゲーション フレームを使用して次のページに進みます
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
        }

        /// <summary>
        /// このページがアクティブで、ウィンドウ全体を使用する場合、Alt キーの組み合わせなどのシステム キーを含む、
        /// キーボード操作で呼び出されます。ページがフォーカスされていないときでも、
        /// ページ間のキーボード ナビゲーションの検出に使用されます。
        /// </summary>
        /// <param name="sender">イベントをトリガーしたインスタンス。</param>
        /// <param name="args">イベントが発生する条件を説明するイベント データ。</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs args)
        {
            var virtualKey = args.VirtualKey;

            // 左方向キーや右方向キー、または専用に設定した前に戻るキーや次に進むキーを押した場合のみ、
            // 詳細を調査します
            if ((args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                args.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // 前に戻るキーまたは Alt キーを押しながら左方向キーを押すと前に戻ります
                    args.Handled = true;
                    this.GoBack(this, new RoutedEventArgs());
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // 次に進むキーまたは Alt キーを押しながら右方向キーを押すと次に進みます
                    args.Handled = true;
                    this.GoForward(this, new RoutedEventArgs());
                }
            }
        }

        /// <summary>
        /// このページがアクティブで、ウィンドウ全体を使用する場合、マウスのクリック、タッチ スクリーンのタップなどの
        /// 操作で呼び出されます。ページ間を移動するため、マウス ボタンのクリックによるブラウザー スタイルの
        /// 次に進むおよび前に戻る操作の検出に使用されます。
        /// </summary>
        /// <param name="sender">イベントをトリガーしたインスタンス。</param>
        /// <param name="args">イベントが発生する条件を説明するイベント データ。</param>
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs args)
        {
            var properties = args.CurrentPoint.Properties;

            // 左、右、および中央ボタンを使用したボタン操作を無視します
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // [戻る] または [進む] を押すと適切に移動します (両方同時には押しません)
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                args.Handled = true;
                if (backPressed) this.GoBack(this, new RoutedEventArgs());
                if (forwardPressed) this.GoForward(this, new RoutedEventArgs());
            }
        }

        #endregion

        #region 表示状態の切り替え

        /// <summary>
        /// イベント ハンドラーとして呼び出されます。これは通常、ページ内の <see cref="Control"/> の
        /// <see cref="FrameworkElement.Loaded"/> イベントで呼び出され、送信元が
        /// アプリケーションのビューステートの変更に対応する表示状態管理の変更を受信開始する必要があることを
        /// 示します。
        /// </summary>
        /// <param name="sender">ビューステートに対応する表示状態管理をサポートする 
        /// <see cref="Control"/> のインスタンス。</param>
        /// <param name="e">要求がどのように行われたかを説明するイベント データ。</param>
        /// <remarks>現在のビューステートは、レイアウトの更新が要求されると、
        /// 対応する表示状態を設定するためすぐに使用されます。対応する 
        /// <see cref="FrameworkElement.Unloaded"/> イベント ハンドラーを
        /// <see cref="StopLayoutUpdates"/> に接続しておくことを強くお勧めします。
        /// <see cref="LayoutAwarePage"/> のインスタンスは、Loaded および Unloaded イベントでこれらのハンドラーを自動的に
        /// 呼び出します。</remarks>
        /// <seealso cref="DetermineVisualState"/>
        /// <seealso cref="InvalidateVisualState"/>
        public void StartLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            if (this._layoutAwareControls == null)
            {
                // 更新の対象となるコントロールがある場合、ビューステートの変更の待機を開始します
                Window.Current.SizeChanged += this.WindowSizeChanged;
                this._layoutAwareControls = new List<Control>();
            }
            this._layoutAwareControls.Add(control);

            // コントロールの最初の表示状態を設定します
            VisualStateManager.GoToState(control, DetermineVisualState(ApplicationView.Value), false);
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.InvalidateVisualState();
        }

        /// <summary>
        /// これは通常、<see cref="Control"/> の
        /// <see cref="FrameworkElement.Unloaded"/> イベントでイベント ハンドラーとして呼び出され、送信元がアプリケーションのビューステートの変更に対応する
        /// ビューステートの変更に対応する表示状態管理の変更を受信開始する必要があることを示します。
        /// </summary>
        /// <param name="sender">ビューステートに対応する表示状態管理をサポートする 
        /// <see cref="Control"/> のインスタンス。</param>
        /// <param name="e">要求がどのように行われたかを説明するイベント データ。</param>
        /// <remarks>現在のビューステートは、レイアウトの更新が要求されると、
        /// 表示状態を設定するためすぐに使用されます。</remarks>
        /// <seealso cref="StartLayoutUpdates"/>
        public void StopLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null || this._layoutAwareControls == null) return;
            this._layoutAwareControls.Remove(control);
            if (this._layoutAwareControls.Count == 0)
            {
                // 更新の対象となるコントロールがない場合、ビューステートの変更の待機を停止します
                this._layoutAwareControls = null;
                Window.Current.SizeChanged -= this.WindowSizeChanged;
            }
        }

        /// <summary>
        /// <see cref="ApplicationViewState"/> 値を、ページ内の表示状態管理で使用できる文字列に
        /// 変換します。既定の実装では列挙値の名前を使用します。
        /// サブクラスでこのメソッドをオーバーライドして、使用されているマップ スキームを制御する場合があります。
        /// </summary>
        /// <param name="viewState">表示状態が必要なビューステート。</param>
        /// <returns><see cref="VisualStateManager"/> の実行に使用される
        /// 表示状態の名前</returns>
        /// <seealso cref="InvalidateVisualState"/>
        protected virtual string DetermineVisualState(ApplicationViewState viewState)
        {
            return viewState.ToString();
        }

        /// <summary>
        /// 適切な表示状態を使用した表示状態の変更を待機しているすべてのコントロールを更新し
        /// ます。
        /// </summary>
        /// <remarks>
        /// 通常、ビューステートが変更されていない場合でも、別の値が返される可能性がある事を知らせるために
        /// <see cref="DetermineVisualState"/> をオーバーライドすることで
        /// 使用されます。
        /// </remarks>
        public void InvalidateVisualState()
        {
            if (this._layoutAwareControls != null)
            {
                string visualState = DetermineVisualState(ApplicationView.Value);
                foreach (var layoutAwareControl in this._layoutAwareControls)
                {
                    VisualStateManager.GoToState(layoutAwareControl, visualState, false);
                }
            }
        }

        #endregion

        #region プロセス継続時間管理

        private String _pageKey;

        /// <summary>
        /// このページがフレームに表示されるときに呼び出されます。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、表示するグループを示します。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // ナビゲーションを通じてキャッシュ ページに戻るときに、状態の読み込みが発生しないようにします
            if (this._pageKey != null) return;

            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            this._pageKey = "Page-" + this.Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // 新しいページをナビゲーション スタックに追加するとき、次に進むナビゲーションの
                // 既存の状態をクリアします
                var nextPageKey = this._pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // ナビゲーション パラメーターを新しいページに渡します
                this.LoadState(e.Parameter, null);
            }
            else
            {
                // ナビゲーション パラメーターおよび保存されたページの状態をページに渡します。
                // このとき、中断状態の読み込みや、キャッシュから破棄されたページの再作成と同じ対策を
                // 使用します
                this.LoadState(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]);
            }
        }

        /// <summary>
        /// このページがフレームに表示されなくなるときに呼び出されます。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、表示するグループを示します。</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            var pageState = new Dictionary<String, Object>();
            this.SaveState(pageState);
            frameState[_pageKey] = pageState;
        }

        /// <summary>
        /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        /// <param name="navigationParameter">このページが最初に要求されたときに
        /// <see cref="Frame.Navigate(Type, Object)"/> に渡されたパラメーター値。
        /// </param>
        /// <param name="pageState">前のセッションでこのページによって保存された状態の
        /// ディクショナリ。ページに初めてアクセスするとき、状態は null になります。</param>
        protected virtual void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
        /// このページに関連付けられた状態を保存します。値は、
        /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
        /// </summary>
        /// <param name="pageState">シリアル化可能な状態で作成される空のディクショナリ。</param>
        protected virtual void SaveState(Dictionary<String, Object> pageState)
        {
        }

        #endregion

        /// <summary>
        /// IObservableMap の実装では、既定のビュー モデルとして使用するため、再入をサポート
        /// しています。 
        /// </summary>
        private class ObservableDictionary<K, V> : IObservableMap<K, V>
        {
            private class ObservableDictionaryChangedEventArgs : IMapChangedEventArgs<K>
            {
                public ObservableDictionaryChangedEventArgs(CollectionChange change, K key)
                {
                    this.CollectionChange = change;
                    this.Key = key;
                }

                public CollectionChange CollectionChange { get; private set; }
                public K Key { get; private set; }
            }

            private Dictionary<K, V> _dictionary = new Dictionary<K, V>();
            public event MapChangedEventHandler<K, V> MapChanged;

            private void InvokeMapChanged(CollectionChange change, K key)
            {
                var eventHandler = MapChanged;
                if (eventHandler != null)
                {
                    eventHandler(this, new ObservableDictionaryChangedEventArgs(change, key));
                }
            }

            public void Add(K key, V value)
            {
                this._dictionary.Add(key, value);
                this.InvokeMapChanged(CollectionChange.ItemInserted, key);
            }

            public void Add(KeyValuePair<K, V> item)
            {
                this.Add(item.Key, item.Value);
            }

            public bool Remove(K key)
            {
                if (this._dictionary.Remove(key))
                {
                    this.InvokeMapChanged(CollectionChange.ItemRemoved, key);
                    return true;
                }
                return false;
            }

            public bool Remove(KeyValuePair<K, V> item)
            {
                V currentValue;
                if (this._dictionary.TryGetValue(item.Key, out currentValue) &&
                    Object.Equals(item.Value, currentValue) && this._dictionary.Remove(item.Key))
                {
                    this.InvokeMapChanged(CollectionChange.ItemRemoved, item.Key);
                    return true;
                }
                return false;
            }

            public V this[K key]
            {
                get
                {
                    return this._dictionary[key];
                }
                set
                {
                    this._dictionary[key] = value;
                    this.InvokeMapChanged(CollectionChange.ItemChanged, key);
                }
            }

            public void Clear()
            {
                var priorKeys = this._dictionary.Keys.ToArray();
                this._dictionary.Clear();
                foreach (var key in priorKeys)
                {
                    this.InvokeMapChanged(CollectionChange.ItemRemoved, key);
                }
            }

            public ICollection<K> Keys
            {
                get { return this._dictionary.Keys; }
            }

            public bool ContainsKey(K key)
            {
                return this._dictionary.ContainsKey(key);
            }

            public bool TryGetValue(K key, out V value)
            {
                return this._dictionary.TryGetValue(key, out value);
            }

            public ICollection<V> Values
            {
                get { return this._dictionary.Values; }
            }

            public bool Contains(KeyValuePair<K, V> item)
            {
                return this._dictionary.Contains(item);
            }

            public int Count
            {
                get { return this._dictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
            {
                return this._dictionary.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this._dictionary.GetEnumerator();
            }

            public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
            {
                int arraySize = array.Length;
                foreach (var pair in this._dictionary)
                {
                    if (arrayIndex >= arraySize) break;
                    array[arrayIndex++] = pair;
                }
            }
        }
    }
}
