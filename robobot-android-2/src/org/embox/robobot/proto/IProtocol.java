package org.embox.robobot.proto;


public interface IProtocol {

	byte[] setControl(int[] control);

	byte[] translateInput(byte[] data);
}
