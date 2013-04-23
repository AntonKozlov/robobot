package org.embox.robobot.ui;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.EditText;
import org.embox.robobot.IControllable;
import org.embox.robobot.IDevice;
import org.embox.robobot.InternetDevice;
import org.embox.robobot.R;
import org.embox.robobot.proto.IProtocol;
import org.embox.robobot.proto.ProtocolPythonSimulator;

import java.io.IOException;
import java.net.InetAddress;
import java.net.Socket;

/**
 * Created with IntelliJ IDEA.
 * User: vloginova
 * Date: 18.04.13
 * Time: 21:46
 * To change this template use File | Settings | File Templates.
 */
public class AddressChooserActivity extends Activity {
    private static final String TAG = AddressChooserActivity.class.getSimpleName();

    private EditText address;
    private EditText port;

    private static IDevice choosedDevice;
    Socket socket;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.address_chooser);
        address = (EditText) findViewById(R.id.address);
        port = (EditText) findViewById(R.id.port);

        findViewById(R.id.connect).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View arg0) {
                tryConnect();
            }
        });
    }

    private void tryConnect(){
        String address;
        int port;
        address = "192.168.33.17";//this.address.getText().toString();
        port = 1806;//Integer.parseInt(this.port.getText().toString());

        try {
            InetAddress inetAddress = InetAddress.getByName(address);
            socket = new Socket(inetAddress, port);
            Log.d(TAG, "connected");
        } catch (IOException e) {
            Log.d(TAG, "connection failed");
            return;
        }

        choosedDevice = createDevice(socket);
        Intent intent = new Intent(AddressChooserActivity.this, ControlActivity.class);
        startActivity(intent);
    }

    private IDevice createDevice(Socket socket) {
        //TODO
        IDevice dev = new InternetDevice("Python", socket);
        return dev;
    }

    public static IDevice getChoosedDevice() {
        return choosedDevice;
    }
}
