package org.embox.robobot.ui;

import org.embox.robobot.DeviceHandler;
import org.embox.robobot.IDevice;

import android.app.Activity;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnTouchListener;
import android.view.Window;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;
import org.embox.robobot.R;

public class ControlActivity extends Activity implements SensorEventListener{
	private IDevice device;
	
	private Button transmitButton;
	private TextView transmitTextView;
	private SensorManager mSensorManager;
	private Sensor mSensorAcceler;
	
	ImageView leftImageView;
	ImageView rightImageView;
	
	ImageView nxtLeftTrackView;
	ImageView nxtBrickView;
	ImageView nxtRightTrackView;
	
	int[] acts = new int[3];
	
	int oldNxtLeftTrack;
	int oldNxtRightTrack;
	
	Boolean needToTransmit = new Boolean(false);
	Boolean canToTransmit = new Boolean(false);

	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
	    super.onCreate(savedInstanceState);
	    requestWindowFeature(Window.FEATURE_INDETERMINATE_PROGRESS);
	    setContentView(R.layout.control);
	    device = SelectActivity.getChoosedDevice();
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
        
    	oldNxtLeftTrack = 0;
    	oldNxtRightTrack = 0;
        nxtLeftTrackView.setImageResource(R.drawable.track_1);
        nxtBrickView.setImageResource(R.drawable.nxt);
        nxtRightTrackView.setImageResource(R.drawable.track_1);
	}
	
	@Override
	protected void onStop() {
		device.close();
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
		synchronized (canToTransmit) {
			if (canToTransmit) {
				synchronized (needToTransmit) {
					needToTransmit = true;
				}
			}
			 
		}
	}
	
	void stopTransmit() {
		synchronized (canToTransmit) {
			if (canToTransmit) {
				synchronized (needToTransmit) {
					needToTransmit = false;
					leftImageView.setImageResource(R.drawable.actuatorp0);
			        rightImageView.setImageResource(R.drawable.actuatorp0);
					device.calibrate(acts);
					device.setControl(acts);
				}
			}
		}
	}
	
	@Override
	public void onSensorChanged(SensorEvent arg0) {
		acts[0] = (int) (arg0.values[0] * 10);
		acts[1] = (int) (arg0.values[1] * 10);
		acts[2] = (int) (arg0.values[2] * 10);
		if (needToTransmit) {
			int[] feedback = device.setControl(acts); 
			setActuatorsView(leftImageView, feedback[0]);
			setActuatorsView(rightImageView, feedback[1]);
			
		}
		
		int nxtLeftTrack = 0;
		int nxtRightTrack = 0;
		if (needToTransmit) {
			if (acts[0] > 0) {
				nxtLeftTrack = (oldNxtLeftTrack + 1) % 3;
			} else if (acts[0] < 0) {
				nxtLeftTrack = (oldNxtLeftTrack + 2) % 3;
			} else {
				nxtLeftTrack = oldNxtLeftTrack;
			}
			oldNxtLeftTrack = nxtLeftTrack;
			
			if (acts[1] > 0) {
				nxtRightTrack = (oldNxtRightTrack + 1) % 3;
			} else if (acts[1] < 0) {
				nxtRightTrack = (oldNxtRightTrack + 2) % 3;
			} else {
				nxtRightTrack = oldNxtRightTrack;
			}
			oldNxtRightTrack = nxtRightTrack;
		}
		setNxtTrackView(nxtLeftTrackView, nxtLeftTrack);
		setNxtTrackView(nxtRightTrackView, nxtRightTrack);
	}

	int[] actsImages = {
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
	
	int [] nxtTrackImages = {
		R.drawable.track_1,
		R.drawable.track_2,
		R.drawable.track_3
	};
	
	private void setNxtTrackView (ImageView imageView, int imageId) {
		imageView.setImageResource(nxtTrackImages[imageId]);
	}
}
