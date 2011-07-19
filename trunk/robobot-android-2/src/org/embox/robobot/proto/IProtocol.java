package org.embox.robobot.proto;

public interface IProtocol {
	byte[] translateOutput(int[] control);
	
	byte[] translateInput(byte[] data);
}
