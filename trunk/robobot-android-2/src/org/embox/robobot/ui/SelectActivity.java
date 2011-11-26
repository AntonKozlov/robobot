package org.embox.robobot.ui;
import org.embox.robobot.R;



import java.util.ArrayList;

import org.embox.robobot.DeviceManager;
import org.embox.robobot.IDevice;
import org.embox.robobot.ScanDeviceHandler;
import org.embox.robobot.transport.ITransport;

import android.app.Activity;
import android.app.Dialog;
import android.bluetooth.BluetoothAdapter;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.ContextMenu;
import android.view.ContextMenu.ContextMenuInfo;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.widget.AdapterView;
import android.widget.AdapterView.AdapterContextMenuInfo;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;


public class SelectActivity extends Activity {
	private static final int REQUEST_BT_ENABLE = 1;
	
	//view items
	private Button mRescanButton;
	private ListView mFoundDevicesList;
	private ArrayAdapter<String> mFoundDeviceAdapter;

	//
	private SharedPreferences preferences;
	private SharedPreferences.Editor preferencesEditor;
	
	//robobot items
	private DeviceManager deviceManager;
	private ScanDeviceHandler mDeviceHandler;
	private ArrayList<IDevice> deviceList = new ArrayList<IDevice>();
	
	
	//TODO move to device manager?
	public static IDevice choosedDevice;
	public static IDevice getChoosedDevice() {
		IDevice dev = choosedDevice;
		choosedDevice = null;
		return dev;
	}

	
	private class DeviceListClickListener implements OnItemClickListener {

		@Override
		public void onItemClick(AdapterView<?> parent, View view, int position,
				long id) {
			//Toast.makeText(getApplicationContext(),deviceList.get(position).getName(),Toast.LENGTH_LONG).show();
			choosedDevice = deviceList.get(position);
			Intent intent = new Intent(SelectActivity.this, ControlActivity.class);
			startActivity(intent);

		}

	}

	/** Called when the activity is first created. */
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
	
		requestWindowFeature(Window.FEATURE_INDETERMINATE_PROGRESS);
		setContentView(R.layout.select);
		mFoundDevicesList = (ListView) findViewById(R.id.found_devices_list);
		mFoundDeviceAdapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1);
		mFoundDevicesList.setAdapter(mFoundDeviceAdapter);
		mFoundDevicesList.setOnItemClickListener(new DeviceListClickListener());
		mRescanButton = (Button) findViewById(R.id.button_scan);

		mDeviceHandler = new ScanDeviceHandler() {
			private String getDisplayString (IDevice device) {
				return device.getName() + " (" + device.getId() + ")";
			}

			@Override 
			protected void scanFinished() {
				stopProgressBar();
			}
			
			@Override
			protected void deviceFound(IDevice device) {
				for (IDevice dev : deviceList) {
					if (dev == device) {
						return;
					}

				}
				String storedDevName = preferences.getString(device.getId(), "");
				if (storedDevName.compareTo("") != 0) {
					device.setName(storedDevName);
				}
				mFoundDeviceAdapter.add(getDisplayString(device));
				deviceList.add(device);
			}
			@Override
			protected void deviceNameChanged(IDevice device) {
				for (int i = 0; i < deviceList.size(); i++) {
					if (deviceList.get(i) == device) {
						String str = mFoundDeviceAdapter.getItem(i);
						mFoundDeviceAdapter.remove(str);
						mFoundDeviceAdapter.insert(getDisplayString(device), i);
					}
				}
			}
			@Override
			protected void deviceNameUserChanged(IDevice dev) {
				preferencesEditor.putString(dev.getId(), dev.getName());
				preferencesEditor.commit();
				deviceNameChanged(dev);
			}
			
			@Override
			protected void requestHwEnable(ITransport transport) {
				Intent intent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
				startActivityForResult(intent, REQUEST_BT_ENABLE);
			}

		};
		
		deviceManager = new DeviceManager((Context) this, mDeviceHandler);
		
		mRescanButton.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View arg0) {

				startIncrementScan();
			}
		});

		registerForContextMenu(mFoundDevicesList);
		preferences = getPreferences(MODE_PRIVATE);
		preferencesEditor = preferences.edit();
		
		startFullScan();

	}
	
	private void startProgressBar() {
		setProgressBarIndeterminateVisibility(true);
		setTitle(R.string.scanning_string);	
	}
	
	private void stopProgressBar() {
		setProgressBarIndeterminateVisibility(false);
		setTitle(R.string.select_string);	
	}
	
	private void startFullScan() {
		startProgressBar();
		deviceManager.startFullScan();
	}
	
	private void startIncrementScan() {
		startProgressBar();
		deviceManager.startIncrementScan();
	}
	
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (requestCode == REQUEST_BT_ENABLE) {
			if (resultCode == RESULT_OK) {
				startFullScan();
			}
		}
	}
	
	@Override
	public void onCreateContextMenu(ContextMenu menu, View v,
			ContextMenuInfo menuInfo) {
		super.onCreateContextMenu(menu, v, menuInfo);
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.select_device_menu, menu);
	}
	
	@Override
	public boolean onContextItemSelected(MenuItem item) {
		AdapterContextMenuInfo info = (AdapterContextMenuInfo) item.getMenuInfo();
		switch (item.getItemId()) {
		case R.id.rename_context:
			IDevice dev = deviceList.get(info.position);
			Dialog newName = new RenameDialog(this, mDeviceHandler, dev, dev.getName()); 
			newName.show();
			return true;
		default:
			break;
				
		}
		return super.onContextItemSelected(item);
	}
	@Override
	protected void onDestroy() {
		deviceManager.stopScan();
		super.onDestroy();
	}
}