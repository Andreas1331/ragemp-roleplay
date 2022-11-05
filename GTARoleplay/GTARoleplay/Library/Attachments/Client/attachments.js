mp.events.addDataHandler("AttachedContainer", (entity, value, oldValue) => {
	if(value === null){
		if(entity.doesExist())
			entity.detach(true, false);
	}else{
		let attachedData = JSON.parse(value);
		attachEntityToEntity(entity, attachedData);
	}
});

function attachEntityToEntity(entity, attachedData){
	if(!attachedData)
		return;

	let attachedToEntity = getAttachedToEntity(attachedData);
	if(!attachedToEntity || !attachedToEntity.doesExist())
		return;	

	const boneIndex = attachedToEntity.getBoneIndexByName(`${attachedData.BoneName}`);
	entity.attachTo(attachedToEntity.handle, boneIndex, attachedData.Offset.x, attachedData.Offset.y, attachedData.Offset.z, 
		attachedData.Rotation.x, attachedData.Rotation.y, attachedData.Rotation.z, 
		attachedData.P9, attachedData.UseSoftPinning, attachedData.Collision, attachedData.IsPed, attachedData.VertexIndex, attachedData.FixedRotation);    
}

function getAttachedToEntity(attachedData){
	if(!attachedData || isNaN(attachedData.AttachedToEntity))
		return null;

	let attachedToEntity = null;
	// Find the attached to object in the corresponding pool
	switch(attachedData.AttachedToType){
		case 2: // Objects
			attachedToEntity = mp.objects.atRemoteId(attachedData.AttachedToEntity);
			break;
	}

	return attachedToEntity;
}
