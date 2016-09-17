﻿package
{
    
    import flash.display.DisplayObject;
    import flash.display.MovieClip;
    import flash.display.StageScaleMode;
    import flash.events.*;
    import flash.external.*;
    import flash.net.URLRequest;
    import flash.display.Loader;
    import flash.events.Event;
    import flash.events.ProgressEvent;
    import flash.text.TextField;
    import scaleform.clik.*;
    import scaleform.clik.events.*;
    import scaleform.clik.controls.*;
    import scaleform.clik.data.*;
    import flash.text.TextFormat;
    
    public class MainMenu extends MovieClip
    {
        public var buttonsListeningForClicks:Vector.<DisplayObject>;
        public var currentScreen:int;
        public var screenHistory:Vector.<int>;
        public var loadedSWF:MovieClip;
        public var loadedXMLData:String;
        public var ButtonStart:MovieClip;
        public var ButtonRTT:MovieClip;
        public var ButtonFonts:MovieClip;
        public var ButtonCLIK:MovieClip;
        public var ButtonBack:MovieClip;
        public var ButtonDemo:MovieClip;
        public var ButtonDocs:MovieClip;
        public var ButtonInteraction:MovieClip;
        public var FontName:DropdownButton;
        public var FontSize:DropdownButton;
        public var GrabbableBG:MovieClip;
        public var CrosshairMC:MovieClip;
        public var InfoText:TextField;
        public var FontText:TextField;
        public var ClikWindow:MovieClip;
        
        public var fontNameData:Array;
        public var fontSizeData:Array;
        public var scaleModeData:Array;
        
        public function MainMenu()
        {
            buttonsListeningForClicks = new Vector.<DisplayObject>();
            screenHistory = new Vector.<int>();
            addEventListener(Event.ENTER_FRAME, configUI);
			stage.scaleMode = "showAll";
        }
        
        public function configUI(e:Event):void
        {
            removeEventListener(Event.ENTER_FRAME, configUI);
            gotoScreen(1);
            addEventListener(Event.ENTER_FRAME, update);
            ExternalInterface.call("OnRegisterSWFCallback", this);
            (this.getChildByName("GrabbableBG") as MovieClip).addEventListener(MouseEvent.MOUSE_DOWN, stageMouseDown);
            
            fontNameData = new Array();
            fontNameData.push({label: "Courier New", index: 1});
            fontNameData.push({label: "Times New Roman", index: 2});
            fontNameData.push({label: "Georgia", index: 3});
            fontNameData.push({label: "", index: 4});
            
            fontSizeData = new Array();
            fontSizeData.push({label: "18", index: 1});
            fontSizeData.push({label: "24", index: 2});
            fontSizeData.push({label: "32", index: 3});
            fontSizeData.push({label: "48", index: 4});
            fontSizeData.push({label: "64", index: 5});
            
            scaleModeData = new Array();
            scaleModeData.push({label: "noBorder", index: 1});
            scaleModeData.push({label: "showAll", index: 2});
            scaleModeData.push({label: "exactFit", index: 3});
            scaleModeData.push({label: "noScale", index: 4});
            scaleModeData.push({label: "", index: 5});
            
			
            if (ClikWindow != null)
            {
                ClikWindow.TabGraphics.addEventListener(MouseEvent.MOUSE_UP, clickGraphics);
                ClikWindow.GraphicsTab.visible = true;
                
                ClikWindow.TabInput.addEventListener(MouseEvent.MOUSE_UP, clickInput);
                ClikWindow.InputTab.visible = false;
                
                if (ClikWindow.GraphicsTab.FieldOfView != null)
                {
                    if (!ClikWindow.GraphicsTab.FieldOfView.hasEventListener(SliderEvent.VALUE_CHANGE))
                    {
                        ClikWindow.GraphicsTab.FieldOfView.addEventListener(SliderEvent.VALUE_CHANGE, changeFieldOfView, false, 0, true);
                    }
                }
                
                if (ClikWindow.GraphicsTab.ScaleMode != null)
                {
                    var dp:DataProvider = new DataProvider(scaleModeData);
                    
                    ClikWindow.GraphicsTab.ScaleMode.dataProvider = dp;
                    ClikWindow.GraphicsTab.ScaleMode.invalidateData();
                    ClikWindow.GraphicsTab.ScaleMode.selectedIndex = 0;
                    ClikWindow.GraphicsTab.ScaleMode.validateNow();
                    
                    if (!ClikWindow.GraphicsTab.ScaleMode.hasEventListener(Event.CHANGE))
                        ClikWindow.GraphicsTab.ScaleMode.addEventListener(Event.CHANGE, changeScaleMode, false, 0, true);
                }
                
                if (ClikWindow.GraphicsTab.DynamicLighting != null)
                {
                    if (!ClikWindow.GraphicsTab.DynamicLighting.hasEventListener(ButtonEvent.CLICK))
                    {
                        ClikWindow.GraphicsTab.DynamicLighting.addEventListener(ButtonEvent.CLICK, toggleDynamicLighting, false, 0, true);
                    }
                }
                
                /*
                if (ClikWindow.GraphicsTab.GameSpeed_Slow != null)
                {
                    if (!ClikWindow.GraphicsTab.GameSpeed_Slow.hasEventListener(ButtonEvent.CLICK))
                    {
                        ClikWindow.GraphicsTab.GameSpeed_Slow.addEventListener(ButtonEvent.CLICK, clickGameSpeedSlow, false, 0, true);
                    }
                }
                
                if (ClikWindow.GraphicsTab.GameSpeed_Medium != null)
                {
                    if (!ClikWindow.GraphicsTab.GameSpeed_Medium.hasEventListener(ButtonEvent.CLICK))
                    {
                        ClikWindow.GraphicsTab.GameSpeed_Medium.addEventListener(ButtonEvent.CLICK, clickGameSpeedMedium, false, 0, true);
                    }
                }
                
                if (ClikWindow.GraphicsTab.GameSpeed_Fast != null)
                {
                    if (!ClikWindow.GraphicsTab.GameSpeed_Fast.hasEventListener(ButtonEvent.CLICK))
                    {
                        ClikWindow.GraphicsTab.GameSpeed_Fast.addEventListener(ButtonEvent.CLICK, clickGameSpeedFast, false, 0, true);
                    }
                }*/
                
                var Group1: ButtonGroup = ButtonGroup.getGroup("GameSpeed", this);
                Group1.addEventListener(Event.CHANGE, handleGroup1Change, false, 0, true);
                
                //
                
                if (ClikWindow.InputTab.Sensitivity != null)
                {
                    if (!ClikWindow.InputTab.Sensitivity.hasEventListener(SliderEvent.VALUE_CHANGE))
                    {
                        ClikWindow.InputTab.Sensitivity.addEventListener(SliderEvent.VALUE_CHANGE, changeSensitivity, false, 0, true);
                    }
                }
                
                if (ClikWindow.InputTab.InvertY != null)
                {
                    if (!ClikWindow.InputTab.InvertY.hasEventListener(ButtonEvent.CLICK))
                    {
                        ClikWindow.InputTab.InvertY.addEventListener(ButtonEvent.CLICK, toggleInvertY, false, 0, true);
                    }
                }
                
                if (ClikWindow.InputTab.FreeLook != null)
                {
                    if (!ClikWindow.InputTab.FreeLook.hasEventListener(ButtonEvent.CLICK))
                    {
                        ClikWindow.InputTab.FreeLook.addEventListener(ButtonEvent.CLICK, toggleFreeLook, false, 0, true);
                    }
                }
            }
            
            ClikWindow.visible = false;
            
            updateClik();
        }
        
        public var theClikWindow: MovieClip;
        
        public function update(e:Event):void
        {
            if (FontText != null)
            {
                FontText.y -= 2;
                if (FontText.y + FontText.height < 0)
                    FontText.y = this.stage.stageHeight;
            }
        }
        
        //public var defaultSFxVolume:Number = 0.5;
        //public var defaultMusicVolume:Number = 0.5;
        //public var defaultSensitivity:Number = 1;
        //public var defaultFieldOfView:Number = 80;
        //public var defaultGameSpeed:Number = 1;
        
        public var defaultFreeLook:Boolean = true;
        public var defaultDynamicLighting:Boolean = true;
        public var defaultInvertY:Boolean = false;
        
        public function handleGroup1Change(e: Event):void
        {
            trace("buttnog roup change ");
        }
        
        public function changeSensitivity(e:Event):void
        {
            trace("changeSensitivity");
            ExternalInterface.call("SetSensitivity", ClikWindow.InputTab.Sensitivity.value);
        }
        
        public function toggleFreeLook(e:Event):void
        {
            trace("toggleFreeLook");
            defaultFreeLook = !defaultFreeLook;
            ExternalInterface.call("SetFreeLook", defaultFreeLook);
        }
        
        public function toggleInvertY(e:Event):void
        {
            trace("toggleInvertY");
            defaultInvertY = !defaultInvertY;
            ExternalInterface.call("SetInvertY", defaultInvertY);
        }
        
        private function changeScaleMode(e:Event):void
        {
            if (ClikWindow.GraphicsTab.ScaleMode != null)
            {
                stage.scaleMode = scaleModeData[ClikWindow.GraphicsTab.ScaleMode.selectedIndex].label as String;
            }
            trace("change scale mode " + stage.scaleMode);
        }
        
        private function changeFieldOfView(e:Event):void
        {
            trace("change field of view");
            ExternalInterface.call("SetFieldOfView", ClikWindow.GraphicsTab.FieldOfView.value);
           // defaultFieldOfView = ClikWindow.GraphicsTab.FieldOfView.value;
        }
        
        private function toggleDynamicLighting(e:Event):void
        {
            trace("toggle dynamic lighting");
            defaultDynamicLighting = !defaultDynamicLighting;
            ExternalInterface.call("SetDynamicLighting", defaultDynamicLighting);
        }
        
        private function clickGameSpeedSlow(e:Event):void
        {
            trace("game speed slow");
        }
        
        private function clickGameSpeedMedium(e:Event):void
        {
            trace("game speed medium");
        }
        
        private function clickGameSpeedFast(e:Event):void
        {
            trace("game speed fast");
        }
        
        private function clickGraphics(e:Event):void
        {
            ClikWindow.GraphicsTab.visible = true;
            ClikWindow.InputTab.visible = false;
            updateClik();
        }
        
        private function clickInput(e:Event):void
        {
            ClikWindow.GraphicsTab.visible = false;
            ClikWindow.InputTab.visible = true;
            updateClik();
        }
        
        
        private function updateClik():void
        {
            trace("updating components");
        }
        
        //////////////////
        
        public function stageMouseDown(e:Event):void
        {
            switch (currentScreen)
            {
                case 10: 
                case 9: 
                case 8: 
                case 7: 
                    break;
                default: 
                    ExternalInterface.call("SetStageMouse", true);
                    stage.addEventListener(MouseEvent.MOUSE_UP, stageMouseUp);
                    break;
            }
        }
        
        public function stageMouseUp(e:Event):void
        {
            ExternalInterface.call("SetStageMouse", false);
            stage.removeEventListener(MouseEvent.MOUSE_UP, stageMouseUp);
        }
        
        public function addListenersToButtons():void
        {
            if (currentScreen == 10)
            {
                if (ClikWindow != null)
                {
                    ClikWindow.visible = true;
                }
                else
                {
                    ClikWindow.visible = false;
                }
            }
            
            for (var a:int = 0; a < this.numChildren; a++)
            {
                var ish:DisplayObject = this.getChildAt(a);
                if (!ish.hasEventListener(MouseEvent.CLICK))
                {
                    switch (ish.name)
                    {
                        case "ButtonStart": 
                            addClickListener(ish, clickStart);
                            break;
                        case "ButtonBack": 
                            addClickListener(ish, clickBack);
                            break;
                        case "ButtonRTT": 
                            addClickListener(ish, clickRTT);
                            break;
                        case "ButtonFonts": 
                            addClickListener(ish, clickFonts);
                            break;
                        case "ButtonInteraction": 
                            addClickListener(ish, clickInteraction);
                            break;
                        case "ButtonCLIK": 
                            addClickListener(ish, clickCLIK);
                            break;
                        case "ButtonDemo": 
                            addClickListener(ish, clickDemo);
                            break;
                        case "ButtonDocs": 
                            addClickListener(ish, clickDocs);
                            break;
                    }
                }
            }
            
            if (FontName != null)
            {
                
                if (!FontName.hasEventListener(Event.CHANGE))
                {
                    FontName.addEventListener(Event.CHANGE, changeFontName, false, 0, true);
                }
                
                var dp:DataProvider = new DataProvider(fontNameData);
                
                FontName.dataProvider = dp;
                FontName.invalidateData();
                FontName.selectedIndex = 0;
                FontName.validateNow();
            }
            
            if (FontSize != null)
            {
                
                if (!FontSize.hasEventListener(Event.CHANGE))
                {
                    FontSize.addEventListener(Event.CHANGE, changeFontSize, false, 0, true);
                }
                
                var dp:DataProvider = new DataProvider(fontSizeData);
                
                FontSize.dataProvider = dp;
                FontSize.invalidateData();
                FontSize.selectedIndex = 0;
                FontSize.validateNow();
            }
        }
        
        private function changeFontName(e:Event):void
        {
            if (FontName != null && FontText != null)
            {
                var myFont:TextFormat = FontText.getTextFormat();
                myFont.font = fontNameData[FontName.selectedIndex].label as String;
                FontText.setTextFormat(myFont)
                FontText.embedFonts = true;
            }
        }
        
        private function changeFontSize(e:Event):void
        {
            if (FontSize != null && FontText != null)
            {
                var myFont:TextFormat = FontText.getTextFormat();
                myFont.size = int(fontSizeData[FontSize.selectedIndex].label as String);
                FontText.setTextFormat(myFont)
                FontText.embedFonts = true;
            }
        }
        
        public function addClickListener(ish:DisplayObject, callback:Function):void
        {
            ish.addEventListener(MouseEvent.CLICK, callback);
            buttonsListeningForClicks.push(ish);
        }
        
        public function gotoScreen(newScreen:int):void
        {
            if (currentScreen == newScreen)
                return;
            
            screenHistory.push(newScreen);
            currentScreen = newScreen;
            gotoAndStop(newScreen);
            addListenersToButtons();
            ExternalInterface.call("SetState", newScreen);
        }
        
        public function clickBack(e:Event):void
        {
            
            ClikWindow.visible = false;
            
            if (screenHistory.length > 1)
            {
                var lastScreen:int = screenHistory.splice(screenHistory.length - 2, 2)[0];
                gotoScreen(lastScreen);
            }
            if (loadedSWF != null)
            {
                loadedSWF.parent.removeChild(loadedSWF);
                loadedSWF = null;
            }
            
            switch (currentScreen)
            {
                case 3: 
                    ExternalInterface.call("CloseGate");
                    break;
            }
        }
        
        public function clickStart(e:Event):void
        {
            gotoScreen(2);
        }
        
        public function clickDemo(e:Event):void
        {
            switch (currentScreen)
            {
                case 3: 
                    ExternalInterface.call("OpenGate");
                    gotoScreen(7);
                    break;
                case 4: 
                    gotoScreen(8);
                    break;
                case 5: 
                    gotoScreen(9);
                    break;
                case 6: 
                    gotoScreen(10);
                    //startLoad("XMLExample.swf");
                    break;
                default: 
                    gotoScreen(7);
                    break;
            }
        
        }
        
        public function clickDocs(e:Event):void
        {
            ExternalInterface.call("GotoURL", "http://gameware.autodesk.com/scaleform/unity/docs");
        }
        
        public function clickRTT(e:Event):void
        {
            gotoScreen(3);
        }
        
        public function clickInteraction(e:Event):void
        {
            gotoScreen(4);
        }
        
        public function clickFonts(e:Event):void
        {
            gotoScreen(5);
        }
        
        public function clickCLIK(e:Event):void
        {
            gotoScreen(6);
        }
        
        ///////////////////
        
        public function ParseXML(data:String):void
        {
            if (currentScreen != 10)
                return;
            
            (loadedSWF.getChildByName("XMLText") as TextField).text = data;
        
        }
        
        public function startLoad(filename:String):void
        {
            var mLoader:Loader = new Loader();
            var mRequest:URLRequest = new URLRequest(filename);
            mLoader.contentLoaderInfo.addEventListener(Event.COMPLETE, onCompleteHandler);
            mLoader.contentLoaderInfo.addEventListener(ProgressEvent.PROGRESS, onProgressHandler);
            mLoader.load(mRequest);
        
        }
        
        public function onCompleteHandler(loadEvent:Event):void
        {
            loadedSWF = loadEvent.currentTarget.content;
            addChild(loadedSWF);
            loadedSWF.y = this.height * .5 - loadedSWF.height * .5;
            loadedSWF.x = 15;
            ExternalInterface.call("LoadXML", "http://area.autodesk.com/blogs/rss/matthew");
        }
        
        public function onProgressHandler(mProgress:ProgressEvent):void
        {
            var percent:Number = mProgress.bytesLoaded / mProgress.bytesTotal;
            trace(percent);
        }
    }

}
