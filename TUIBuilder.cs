using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace TUIBuilderNET
{
    public class TUIBuilder
    {
        public int selection = 0;
        private int _sizex,_sizey = 0;
        private bool quit = false;
        public Dictionary<string, object> colors = new Dictionary<string, object>();
        private List<TObject> selectable = new List<TObject>();
        private int lastid = 0;
        private Thread mainthread;
        public List<TObject> map = new List<TObject>();
        public Action<ConsoleKeyInfo> onKeyPress = null;
        public enum ObjectType
        {
            TEXT,
            FTEXT,
            BUTTON,
            SLIDER,
            INPUT,
            PICKER,
            CHMARK/*,
            CANVAS*/
        }
        public enum SliderType
        {
            BLOCK,
            BAR,
            NUMBER
        }
        public enum PickerType
        {
            LIST,
            RADIO
        }

        public TUIBuilder(ConsoleColor select1color = ConsoleColor.DarkBlue, ConsoleColor select2color = ConsoleColor.Blue, object textcolor = null, object bgcolor = null, ConsoleColor placecolor = ConsoleColor.Gray)
        {
            colors.Add("s1c", select1color);
            colors.Add("s2c", select2color);
            colors.Add("tc", textcolor);
            colors.Add("bc", bgcolor);
            colors.Add("pc", placecolor);
        }


        public class TObject
        {
            public int id;
            public ObjectType type;
            public bool canBeSelected;
            public Action onSelect = null;
            public object Forecolor = null;
            public TObject(ObjectType t)
            {
                type = t;
                canBeSelected = checkSelect(t);
            }
            private bool checkSelect(ObjectType t)
            {
                ObjectType[] selectableTypes = { ObjectType.BUTTON, ObjectType.SLIDER, ObjectType.INPUT, ObjectType.PICKER, ObjectType.CHMARK };
                return Array.IndexOf(selectableTypes, t) != -1;
            }

        }
        public class Text : TObject
        {

            public string content;
            public Text(string text) : base(ObjectType.TEXT)
            {
                content = text;

            }

        }
        public class FText : TObject
        {
            public Func<object> content;
            public FText(Func<object> ret) : base(ObjectType.FTEXT)
            {
                content = ret;
            }

        }
        public class Button : TObject
        {

            public string content;
            public Action onclick;
            public Button(string text, Action func) : base(ObjectType.BUTTON)
            {
                content = text;
                onclick = func;
            }
        }
        public class Input : TObject
        {

            public string content;
            public string placeholder;
            public bool multiline;
            public int fixedlength;
            public Input(string phold = "", bool multi = false, int fixedlen = 0) : base(ObjectType.INPUT)
            {
                fixedlength = fixedlen;
                placeholder = phold;
                multiline = multi;
            }
        }
        public class Slider : TObject
        {
            public List<string> props = new List<string>();
            
            public int value;
            public int minvalue, maxvalue;
            public SliderType slidertype;
            public Slider(SliderType type = SliderType.BAR, int min = 0, int max = 5, int start = 0) : base(ObjectType.SLIDER)
            {
                props.Add("NONE");
                if(type == SliderType.BLOCK){
                    props = new List<string>();
                    props.Add("▁▂▃▄▅▆▇█");
                }
                if(type == SliderType.BAR){
                    props = new List<string>();
                    props.Add("-");
                    props.Add("o");
                }
                

                if(max > props[0].Length && type == SliderType.BLOCK) max = props[0].Length;
                if(start > max) start = max;
                if(start < min) start = min;

                maxvalue = max;
                minvalue = min;
                value = start;
                slidertype = type;
            }
        }
        public class Picker : TObject
        {
            public object[] list;
            public int selection = 0;
            public PickerType pickertype;
            public Picker(IEnumerable<object> options, PickerType pt = PickerType.LIST) : base(ObjectType.PICKER)
            {
                list = (object[])options;
                pickertype = pt;
            }

        }
        public class Checkmark : TObject
        {
            public bool ischecked;
            public List<string> props = new List<string>();
            public Checkmark(bool startchecked = false) : base(ObjectType.CHMARK)
            {
                ischecked = startchecked;
                props.Add("v");
            }

        }/*
        public class Canvas : TObject
        {

            public List<List<char>> content;
            public bool showBorder, wordWrap, scrolling;
            public int X, Y;
            private int cX, cY = 0;
            public Canvas(int x = 6, int y = 6, bool border = true, bool wrap = true, bool scroll = false) : base(ObjectType.CANVAS)
            {
                showBorder = border;
                wordWrap = wrap;
                scrolling = scroll;
                X = x;
                Y = y;
                content = new List<List<char>>(new List<char>[Y]);
                for(int i = 0; i<Y; i++){
                    content[i] = new List<char>(new char[X]);
                }
            }
            private void _write(object m)
            {
                foreach(char c in m.ToString()){
                    if(cX >= X){
                        cX = 0;
                        cY++;
                    }
                    if(cY >= Y && !scrolling) continue;
                    if(c == '\n'){
                        cY++;
                        cX = 0;
                        continue;
                    }
                    if(c == '\r'){
                        cX = 0;
                        continue;
                    }
                    if(c == '\b' || c == 127){
                        if(cX == 0 && cY == 0) continue;
                        if(cX == 0 && cY > 0) {
                            cX = X;
                            cY--;
                        }
                        cX--;
                        content[cY][cX] = '\0';
                        continue;
                    }
                    if(cY >= Y){
                        content.RemoveAt(0);
                        content.Add(new List<char>(new char[X]));
                        cY = Y-1;
                    }

                    content[cY][cX++] = c;
                }
            }
            private string _addspace(string[] w, string word){
                return (w.Length == 1) || Array.IndexOf(w, word) == (w.Length-1) ? "" : " ";
            }
            public void Write(object m){


                foreach(string word in m.ToString().Split(" ")){
                    if(!wordWrap){
                        _write(word + _addspace(m.ToString().Split(" "), word));
                        continue;
                    }

                    if(word.Length <= (X - cX)){
                        _write(word + _addspace(m.ToString().Split(" "), word));
                    } else {
                        _write("\n" + word + _addspace(m.ToString().Split(" "), word));
                    }
                }
            }
            public void WriteLine(object m){
                Write(m);
                cY++;
                cX = 0;
            }
            public void Plot(char m, int x, int y){
                if(x>X || y>Y) return;
                content[y][x] = m;
            }
            public void Clear(){
                content = new List<List<char>>(new List<char>[X]);
                for(int i = 0; i<Y; i++){
                    content[i] = new List<char>(new char[X]);
                }
                cX = 0;
                cY = 0;
            }
            

        }*/


        public void Render()
        {
            if(colors.GetValueOrDefault("bc") != null){
                Console.BackgroundColor = (ConsoleColor)colors.GetValueOrDefault("bc");
            }
            Console.Clear();
            foreach (TObject i in map)
            {
                if(colors.GetValueOrDefault("tc") != null){
                    Console.ForegroundColor = (ConsoleColor)colors.GetValueOrDefault("tc");
                }
                if(colors.GetValueOrDefault("bc") != null){
                    Console.BackgroundColor = (ConsoleColor)colors.GetValueOrDefault("bc");
                }
                
                
                if(i.Forecolor != null){
                    Console.ForegroundColor = (ConsoleColor)i.Forecolor;
                }


                if (i.type == ObjectType.TEXT)
                {
                    Console.Write(((TUIBuilder.Text)i).content);
                }
                if (i.type == ObjectType.FTEXT)
                {
                    Console.Write(((TUIBuilder.FText)i).content());
                }
                if (i.type == ObjectType.BUTTON)
                {
                    var j = (TUIBuilder.Button)i;
                    if (CheckSelect(i))
                    {
                        Console.BackgroundColor = (ConsoleColor)colors.GetValueOrDefault("s1c");
                        Console.Write(j.content);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(j.content);
                    }
                }
                if (i.type == ObjectType.INPUT)
                {
                    var j = (TUIBuilder.Input)i;
                    var content = j.content + "";
                    content = content.Replace("\n", "\n  ");

                    void _render(){
                        if(content == "") {
                            if(j.fixedlength != 0) 
                                content = new string(' ', (j.fixedlength - j.placeholder.Length) / 2) + j.placeholder;
                            else
                                content = j.placeholder;
                        }
                        if (content.Length < j.fixedlength)
                            content = content + new string(' ', j.fixedlength - content.Length);
                        if (j.fixedlength > 0){
                            if(j.multiline){
                                content = content.Split("\n  ")[content.Split("\n  ").Length-1];

                                if (content.Length < j.fixedlength)
                                    content = content + new string(' ', j.fixedlength - content.Length);
                                    
                                content = content.Substring(content.Length - j.fixedlength);
                            } else {
                                content = content.Substring(content.Length - j.fixedlength);
                            }
                        }
                        Console.Write("[ " + content + " ]");
                    }
                    if (CheckSelect(i))
                    {
                        Console.BackgroundColor = (ConsoleColor)colors.GetValueOrDefault("s1c");
                        _render();
                        Console.ResetColor();
                    }
                    else
                    {
                        _render();
                    }


                }
                if (i.type == ObjectType.SLIDER)
                {
                    var j = (TUIBuilder.Slider)i;

                    void _render(SliderType type){
                        if(type == SliderType.BLOCK){
                            Console.Write(" " + j.props[0].Substring(0, j.value));
                        }
                        if(type == SliderType.BAR){
                            var final = new StringBuilder(new String(j.props[0].ToCharArray()[0], j.maxvalue+1));
                            final[j.value] = j.props[1].ToCharArray()[0];
                            Console.Write(final);
                        }
                        if(type == SliderType.NUMBER){
                            Console.Write(j.value);
                        }
                    }

                    if (CheckSelect(i))
                    {
                        Console.BackgroundColor = (ConsoleColor)colors.GetValueOrDefault("s1c");
                        _render(j.slidertype);
                        Console.ResetColor();
                    }
                    else
                    {
                        _render(j.slidertype);
                    }
                }

                if (i.type == ObjectType.PICKER)
                {
                    var j = (TUIBuilder.Picker)i;

                    void _render(PickerType type, bool selected = false){
                        if(type == PickerType.LIST){
                            Console.Write("{ " + j.list[j.selection] + " }");
                        }
                        if(type == PickerType.RADIO){
                            foreach (object o in j.list){
                                if(o == j.list[j.selection]){
                                    if(selected) Console.BackgroundColor = (ConsoleColor)colors.GetValueOrDefault("s2c");
                                    Console.Write(o + " (o)");
                                    if(selected) Console.BackgroundColor = (ConsoleColor)colors.GetValueOrDefault("s1c");
                                    if(o != j.list[j.list.Length-1]) Console.Write(" ");
                                } else {
                                    Console.Write(o + " ( )");
                                    if(o != j.list[j.list.Length-1]) Console.Write(" ");
                                }
                            }
                        }
                    }

                    if (CheckSelect(i))
                    {
                        Console.BackgroundColor = (ConsoleColor)colors.GetValueOrDefault("s1c");
                        _render(j.pickertype, true);
                        Console.ResetColor();
                    }
                    else
                    {
                        _render(j.pickertype);
                    }
                }

                 if (i.type == ObjectType.CHMARK)
                {
                    var j = (TUIBuilder.Checkmark)i;

                    if (CheckSelect(i))
                    {
                        Console.BackgroundColor = (ConsoleColor)colors.GetValueOrDefault("s1c");
                        Console.Write("(" + (j.ischecked ? j.props[0] : " ") + ")");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("(" + (j.ischecked ? j.props[0] : " ") + ")");
                    }
                }

                /*
                if (i.type == ObjectType.CANVAS)
                {
                    var canv = ((TUIBuilder.Canvas)i);
                    string finished = "";
                    if(canv.showBorder){
                        finished += "+" + new string('-', canv.X) + "+\n";
                        for(int k = 0; k<canv.Y; k++){
                            finished += "|";
                            for(int l = 0; l<canv.X; l++){
                                if(canv.content[k][l] == 0){
                                    finished += " ";
                                } else {
                                    finished += canv.content[k][l];
                                }

                            }
                            finished += "|\n";
                        }
                        finished += "+" + new string('-', canv.X) + "+";
                    } else {
                        for(int k = 0; k<canv.Y; k++){
                            for(int l = 0; l<canv.X; l++){
                                if(canv.content[k][l] == 0){
                                    finished += " ";
                                } else {
                                    finished += canv.content[k][l];
                                }

                            }
                            finished += "\n";
                        }
                    }
                    Console.Write(finished);
                }*/

                

            }
        }
        private void MainTask()
        {
            while (!quit)
            {
                Render();
                var key = Console.ReadKey();

                Thread.Sleep(10);

                if(onKeyPress != null)
                    onKeyPress(key);


                if(GetSelect(selection) == null) continue;
                
                if(GetSelect(selection).onSelect != null){
                    GetSelect(selection).onSelect();
                }
                

                // interaction with selectable objects
                if (GetSelect(selection).type == ObjectType.BUTTON)
                {
                    if (key.Key == ConsoleKey.Enter)
                    {
                        ((TUIBuilder.Button)(GetSelect(selection))).onclick();
                    }
                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        ((TUIBuilder.Button)(GetSelect(selection))).onclick();
                    }
                }
                if(GetSelect(selection) == null) continue;

                if (GetSelect(selection).type == ObjectType.INPUT)
                {
                    string content = ((TUIBuilder.Input)(GetSelect(selection))).content;
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Tab && key.KeyChar > 31)
                    {
                        content += key.KeyChar;
                    }
                    if (key.Key == ConsoleKey.Enter && ((TUIBuilder.Input)(GetSelect(selection))).multiline)
                    {
                        content += "\n";
                    }
                    if (key.Key == ConsoleKey.Backspace && content.Length > 0)
                    {
                        content = content.Substring(0, content.Length - 1);
                    }
                    ((TUIBuilder.Input)(GetSelect(selection))).content = content;
                }
                if (GetSelect(selection).type == ObjectType.SLIDER)
                {
                    var slider = ((TUIBuilder.Slider)(GetSelect(selection)));
                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        if(slider.value < slider.maxvalue)
                            ((TUIBuilder.Slider)(GetSelect(selection))).value++;
                    }
                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        if(slider.value > slider.minvalue)
                            ((TUIBuilder.Slider)(GetSelect(selection))).value--;
                    }
                }
                if (GetSelect(selection).type == ObjectType.PICKER)
                {
                    var picker = ((TUIBuilder.Picker)(GetSelect(selection)));
                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        if(picker.selection < (picker.list.Length-1))
                            ((TUIBuilder.Picker)(GetSelect(selection))).selection++;
                    }
                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        if(picker.selection > 0)
                            ((TUIBuilder.Picker)(GetSelect(selection))).selection--;
                    }
                }
                if (GetSelect(selection).type == ObjectType.CHMARK)
                {
                    var checkmark = ((TUIBuilder.Checkmark)(GetSelect(selection)));
                    if (key.Key == ConsoleKey.Enter)
                    {
                        ((TUIBuilder.Checkmark)(GetSelect(selection))).ischecked = !(((TUIBuilder.Checkmark)(GetSelect(selection))).ischecked);
                    }
                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        ((TUIBuilder.Checkmark)(GetSelect(selection))).ischecked = true;
                    }
                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        ((TUIBuilder.Checkmark)(GetSelect(selection))).ischecked = false;
                    }
                }
                
                if (key.Key == ConsoleKey.UpArrow && selection > 0)
                    selection--;
                if (key.Key == ConsoleKey.DownArrow && selection < (selectable.Count - 1))
                    selection++;
                if (key.Key == ConsoleKey.Tab){
                    if(selection < selectable.Count)
                        selection++;
                    if(selection == selectable.Count)
                        selection = 0;
                }
            }
        }
        private void RenderOnResizeTask(){
            while(!quit){
                _sizex = Console.WindowWidth;
                _sizey = Console.WindowHeight;
                Thread.Sleep(300);
                if(_sizex != Console.WindowWidth || _sizey != Console.WindowHeight){
                    Render();
                }

                
            }
        }
        public void Show(bool async = false)
        {
            quit = false;
            Thread thread = new Thread(MainTask);
            mainthread = thread;
            mainthread.Start();
            new Thread(RenderOnResizeTask).Start();
            if (!async) mainthread.Join();
        }
        public void Quit()
        {
            quit = true;
        }
        

        public TObject GetSelect(int s)
        {
            try
            {
                return selectable[s];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool CheckSelect(TObject obj)
        {
            bool result = false;
            try
            {
                result = selectable[selection] == obj;
            }
            catch (Exception) { return false; }
            return result;
        }


        public TObject AddObject(TObject obj, int pos = -1)
        {
            obj.id = lastid++;
            if(pos > -1)
                map.Insert(pos, obj);
            else
                map.Add(obj);

            List<TObject> s = new List<TObject>();
            foreach (TObject i in map)
            {
                if (i.canBeSelected)
                    s.Add(i);
            }
            selectable = s;
            return obj;
        }

        public void Flush(){
            selectable = new List<TObject>();
            map = new List<TObject>();
            Render();
        }

        public void Reset(){
            selectable = new List<TObject>();
            map = new List<TObject>();
            selection = 0;
            _sizex = 0;
            _sizey = 0;
            quit = false;
            colors = new Dictionary<string, object>();
            lastid = 0;
            mainthread = null;
            onKeyPress = null;
            Console.ResetColor();
            Render();
            
        }
    }

}
