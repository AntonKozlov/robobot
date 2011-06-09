package org.embox.robobot.ui;

import org.embox.robobot.DeviceHandler;
import org.embox.robobot.IDevice;

import org.embox.robobot.ui.R;
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
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;

public class ControlActivity extends Activity implements SensorEventListener{
	private IDevice device;
	private Button transmitButton;
	
	private SensorManager mSensorManager;
	private Sensor mSensorAcceler;
	
	ImageView leftImageView;
	ImageView rightImageView;
	
	float curX;
	float curY;
	float curZ;
	
	float callbX;
	float callbY;
	float callbZ;
	
	int[] acts = new int[3];
	int[] zeroControl = new int[3];
	
	boolean needToTransmit;
	
	private float calibrate;
	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
	    super.onCreate(savedInstanceState);
	    setContentView(R.layout.control);
	    device = SelectActivity.getChoosedDevice();
	    Toast.makeText(getApplicationContext(),
	    		device.getName(),Toast.LENGTH_SHORT).show();
	    device.setDeviceHandler(new ControlDeviceHandler());
	    device.init();
	    transmitButton = (Button) findViewById(R.id.button_transmit);
	    transmitButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View v) {
				doCallibrate();
			}
		});
	    
	    transmitButton.setOnTouchListener(new OnTouchListener() {
			
			@Override
			public boolean onTouch(View arg0, MotionEvent arg1) {
				switch (arg1.getAction()) {
				case MotionEvent.ACTION_DOWN:
					doCallibrate();
					doTransmit();
					break;
				case MotionEvent.ACTION_UP:
					stopTransmit();
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
	}
	
	@Override
	protected void onStop() {
		device.close();
		Toast.makeText(getApplicationContext(), "Closing", Toast.LENGTH_SHORT).show();
		super.onStop();
	}
	
	private class ControlDeviceHandler extends DeviceHandler {
		@Override
		protected void initOk() {
			Toast.makeText(getApplicationContext(), "Init OK", Toast.LENGTH_SHORT).show();
			device.connect();
		}
		@Override
		protected void connectOk() {
			Toast.makeText(getApplicationContext(), "Connect OK", Toast.LENGTH_SHORT).show();
			
		}
		@Override
		protected void writeDone() {
			
			//if (needToTransmit) {
			//	device.setControl(acts);
			//}
		}
		
		@Override
		protected void connectError(String error) {
			Toast.makeText(getApplicationContext(), "Connect Error: ".concat(error), Toast.LENGTH_SHORT).show();
		}
		
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

	static private float angle (float x, float y) {
		return (float) Math.acos(x) * Math.signum(y); 
	}
	
	private float cut(float val, float range) {
		if (val < -range) {
			return -range;
		}
		if (val > range) {
			return range;
		}
		return val;
	}
	
	void doCallibrate() {
		calibrate = angle(curX, curZ);
	}
	void actuators(float x, float y, float z) {
		float fullY = (float) 0.3;
		float fullX = (float) 0.7;
		Float power = 100 * cut((angle(x,z) - calibrate) / fullX, 1);
		Float mA = cut(power + 100 * (y / fullY), 100);
		Float mB = cut(power - 100 * (y / fullY), 100);
		acts[0] = mA.intValue();
		acts[1] = mB.intValue();

	}
	
	void doTransmit() {
		needToTransmit = true;
		device.setControl(acts);
	}
	
	void stopTransmit() {
		needToTransmit = false;
		device.setControl(zeroControl);
	}
	
	@Override
	public void onSensorChanged(SensorEvent arg0) {
		curX = (float) (arg0.values[0] * 0.1);
		curY = (float) (arg0.values[1] * 0.1);
		curZ = (float) (arg0.values[2] * 0.1);
		actuators(curX, curY, curZ);
		setActuatorsView(leftImageView, acts[0]);
		setActuatorsView(rightImageView, acts[1]);
		if (needToTransmit) {
			device.setControl(acts);
		}
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
}
