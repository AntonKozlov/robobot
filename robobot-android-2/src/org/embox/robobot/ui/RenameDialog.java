package org.embox.robobot.ui;

import org.embox.robobot.IDevice;
import org.embox.robobot.R;

import org.embox.robobot.ScanDeviceHandler;
import org.embox.robobot.transport.ITransport;

import android.app.Dialog;
import android.content.Context;
import android.os.Handler;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

public class RenameDialog extends Dialog {
	Handler handler;
	IDevice device;
	String initVal;
	private EditText newName;
	private Button okButton;
	private Button cancelButton;
	
	public RenameDialog(Context context, ScanDeviceHandler handler, IDevice device, String string) {
		super(context);
		this.handler = handler;
		this.device = device;
		this.initVal = string;
	}
	
	public RenameDialog(Context context) {
		super(context);

	}
	
	@Override
	protected void onStart() {
		super.onStart();
		super.setTitle(R.string.rename_request);
		super.setContentView(R.layout.rename_dialog);
		
		newName = (EditText) findViewById(R.id.new_name_input);
		newName.setText(initVal);
		
		okButton = (Button) findViewById(R.id.ok_new_name);
		cancelButton = (Button) findViewById(R.id.cancel_new_name);
		
		newName.addTextChangedListener(new TextWatcher() {
			
			@Override
			public void onTextChanged(CharSequence s, int start, int before, int count) {
			}
			
			@Override
			public void beforeTextChanged(CharSequence s, int start, int count,
					int after) {
			}
			
			@Override
			public void afterTextChanged(Editable s) {
				if (s.length() != 0) {
					okButton.setEnabled(true);
				} else {
					okButton.setEnabled(false);
				}
			}
		});
		okButton.setOnClickListener(new View.OnClickListener() {
			
			@Override
			public void onClick(View v) {
				device.setName(newName.getText().toString());
				handler.obtainMessage(ITransport.DEVICE_NAME_USER_CHANGED,device).sendToTarget();
				dismiss();
			}
		});
		
		cancelButton.setOnClickListener(new View.OnClickListener() {
			
			@Override
			public void onClick(View v) {
				dismiss();	
			}
		});
	}

}
