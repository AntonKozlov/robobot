package org.embox.robobot.ui;

import org.embox.robobot.DeviceHandler;
import org.embox.robobot.IDevice;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.Toast;

public class ControlActivity extends Activity {
	private IDevice device;
	private Button transmitButton;
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
				int[] control = new int[1];
				control[0] = 31337;
				device.setControl(control);
			}
		});
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
	}
}
