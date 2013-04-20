package org.embox.robobot.ui;

import org.embox.robobot.DeviceHandler;
import org.embox.robobot.IDevice;
import org.embox.robobot.R;

import android.app.Activity;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnTouchListener;
import android.view.Window;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

public class ControlActivity extends Activity implements SensorEventListener{
	private Button transmitButton;
	private TextView transmitTextView;

	private LinearLayout visualisationLayout;
	
	private ImageView leftImageView;
	private ImageView rightImageView;
	
	private ImageView nxtLeftTrackView;
	private ImageView nxtBrickView;
	private ImageView nxtRightTrackView;
	
	//sensors
	private SensorManager mSensorManager;
	private Sensor mSensorAcceler;
	
	//robobot item
	private IDevice device;
	
	
	int[] acts = new int[3];

	private int oldNxtLeftTrack = 0;
	private int oldNxtRightTrack = 0;
	
	private Boolean needToTransmit = new Boolean(false); // synchronization variable for pressing button controlling
	private Boolean canToTransmit = new Boolean(false); // synchronization variable for connection

	OnClickListener showSensorsListener = new OnClickListener() {
		
		@Override
		public void onClick(View v) {
			//visualisationLayout.setVisibility(View.INVISIBLE);
			return;
		}
	}; 
	
	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
	    super.onCreate(savedInstanceState);
	    requestWindowFeature(Window.FEATURE_INDETERMINATE_PROGRESS);
	    setContentView(R.layout.control);
	    device = SelectActivity.getChoosedDevice() == null ?
                AddressChooserActivity.getChoosedDevice() : SelectActivity.getChoosedDevice();
	    device.setDeviceHandler(new ControlDeviceHandler());
	    device.init();
	    
	    transmitTextView = (TextView) findViewById(R.id.transmit_textview);
	    transmitButton = (Button) findViewById(R.id.button_transmit);
	    transmitButton.setOnTouchListener(new OnTouchListener() {
			
			@Override
			public boolean onTouch(View arg0, MotionEvent arg1) {
				switch (arg1.getAction()) {
				case MotionEvent.ACTION_DOWN:
					doCallibrate();
					doTransmit();
					setTransmitHigh();
					
					break;
				case MotionEvent.ACTION_UP:
					stopTransmit();
					setTransmitLow();
					
				default:
					break;
				}
				return false;
			}
		});

	    mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
        mSensorAcceler = mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        
        leftImageView = (ImageView) findViewById(R.id.image_view_left);
        rightImageView = (ImageView) findViewById(R.id.image_view_right);
        
        leftImageView.setImageResource(R.drawable.actuatorp0);
        rightImageView.setImageResource(R.drawable.actuatorp0);
        
    	nxtLeftTrackView = (ImageView) findViewById(R.id.image_nxt_left_track);
    	nxtBrickView = (ImageView) findViewById(R.id.image_nxt_brick);
    	nxtRightTrackView = (ImageView) findViewById(R.id.image_nxt_right_track);

    	visualisationLayout = (LinearLayout) findViewById(R.id.visualisation_layout);
    	
    	leftImageView.setOnClickListener(showSensorsListener);
    	rightImageView.setOnClickListener(showSensorsListener);
    	nxtLeftTrackView.setOnClickListener(showSensorsListener);
    	nxtRightTrackView.setOnClickListener(showSensorsListener);
    	nxtBrickView.setOnClickListener(showSensorsListener);
    	
        nxtLeftTrackView.setImageResource(R.drawable.track_1);
        nxtBrickView.setImageResource(R.drawable.nxt);
        nxtRightTrackView.setImageResource(R.drawable.track_1);
	}
	
	@Override
	protected void onStop() {
		device.disconnect();
		//Toast.makeText(getApplicationContext(), "Closing", Toast.LENGTH_SHORT).show();
		super.onStop();
	}
	
	private class ControlDeviceHandler extends DeviceHandler {
		@Override
		protected void initOk() {
			//Toast.makeText(getApplicationContext(), "Init OK", Toast.LENGTH_SHORT).show();
			setProgressBarIndeterminateVisibility(true);
			setTitle(R.string.connecting_string);
			device.connect();
		}
		@Override
		protected void connectOk() {
			setProgressBarIndeterminateVisibility(false);
			setTitle(getString(R.string.connected_string) + " " + device.getName());
			//Toast.makeText(getApplicationContext(), "Connect OK", Toast.LENGTH_SHORT).show();
			synchronized (canToTransmit) {
				canToTransmit = true;
			}
		}
		@Override
		protected void writeDone() {
			
		}
		
		@Override
		protected void connectError(String error) {
			Toast.makeText(getApplicationContext(), "Connect Error: ".concat(error), Toast.LENGTH_LONG).show();
			finish();
		}
		
	}

	private void setTransmitHigh() {
		//transmitTextView.setTextColor(R.color.transmit_textview_text_high);
		transmitTextView.setBackgroundResource(R.color.transmit_textview_back_high);
	}
	
	private void setTransmitLow() {
		//transmitTextView.setTextColor(R.color.transmit_textview_text_low);
		transmitTextView.setBackgroundResource(R.color.transmit_textview_back_low);
		
	}

	@Override
	protected void onResume() {
		 mSensorManager.registerListener(this, mSensorAcceler, SensorManager.SENSOR_DELAY_GAME);
	     //mWakeLock.acquire();
		 super.onResume();
	}
	
	@Override
	protected void onPause() {
		mSensorManager.unregisterListener(this);	
		super.onPause();
	}
	@Override
	public void onAccuracyChanged(Sensor arg0, int arg1) {
		
	}

	void doCallibrate() {
		device.calibrate(acts);
	}
	
	void doTransmit() {
		if (canToTransmit) {
			needToTransmit = true;
		}
	}
	
	void stopTransmit() {
		needToTransmit = false;
		leftImageView.setImageResource(R.drawable.actuatorp0);
        rightImageView.setImageResource(R.drawable.actuatorp0);
		device.calibrate(acts);
		device.setControl(acts);
	}
	
	@Override
	public void onSensorChanged(SensorEvent arg0) {
		//TODO define constant? may be calibrate sensitivity?
		acts[0] = (int) (arg0.values[0] * 10);
		acts[1] = (int) (arg0.values[1] * 10);
		acts[2] = (int) (arg0.values[2] * 10);
		if (needToTransmit) {
			int[] feedback = device.setControl(acts); 
			setActuatorsView(leftImageView, feedback[0]);
			setActuatorsView(rightImageView, feedback[1]);
			drawTracks(feedback);	
		}
		
	}

	//TODO constant images?
	private int[] actsImages = {
			R.drawable.actuatorm5,
			R.drawable.actuatorm4,
			R.drawable.actuatorm3,
			R.drawable.actuatorm2,
			R.drawable.actuatorm1,
			R.drawable.actuatorp0,
			R.drawable.actuatorp1,
			R.drawable.actuatorp2,
			R.drawable.actuatorp3,
			R.drawable.actuatorp4,
			R.drawable.actuatorp5
	};
	
	private void setActuatorsView(ImageView imageView, int power) {
		imageView.setImageResource(actsImages[5 + (power / 20)]);
		
	}
	//TODO constant images?
	private int [] nxtTrackImages = {
		R.drawable.track_1,
		R.drawable.track_2,
		R.drawable.track_3
	};	
	
	int nxtTrackImagesLen = nxtTrackImages.length;
	
	private int setNxtTrackView (ImageView imageView, int old, int feedback) {
		int diff = feedback > 0 ? 1 : (feedback < 0 ? -1 : 0);
		int n = (old + diff + nxtTrackImagesLen) % nxtTrackImagesLen;
		imageView.setImageResource(nxtTrackImages[n]);
		return n;
	}
	
	private void drawTracks(int[] feedback) {
		oldNxtLeftTrack = setNxtTrackView(nxtLeftTrackView, 
				oldNxtLeftTrack, feedback[0]);
		oldNxtRightTrack = setNxtTrackView(nxtRightTrackView, 
				oldNxtRightTrack, feedback[1]);
							
	}
}
