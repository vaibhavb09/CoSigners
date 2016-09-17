package  {
	
	import flash.display.MovieClip;
	import flash.events.*;
    import flash.external.*;
	
	public class RenderTextureDemo extends MovieClip {
		
		public var GateMC: MovieClip;
		public function RenderTextureDemo() {
            addEventListener(Event.ENTER_FRAME, configUI);
		}
        
        public function configUI(e: Event):void
        {
            removeEventListener(Event.ENTER_FRAME, configUI);
            ExternalInterface.call("OnRegisterSWFCallback", this);
			
			GateMC = this.getChildByName("GateMC") as MovieClip;
        }
		
		public function openGate():void
		{
			GateMC.gotoAndPlay("open");
		}
        
		public function closeGate():void
		{
			GateMC.gotoAndPlay("close");
		}
	}
	
}
