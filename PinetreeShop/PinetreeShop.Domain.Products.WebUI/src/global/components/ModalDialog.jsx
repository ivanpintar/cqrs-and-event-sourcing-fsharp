import { Modal, Button } from 'react-bootstrap';
import React from 'react';

const ModalDialog = ({ onClose, onSave, saveText, closeText, show, title, children }) => {
    const close = () => {
        if(onClose) onClose();
    };

    const save = () => {
        if(onSave) onSave();
    };

    let saveButton = null;
    if(onSave) {
        saveText = saveText || 'Save';
        saveButton = <Button bsStyle='primary' onClick={save}>{saveText}</Button>;
    };

    closeText = closeText || 'Close';
    let closeButton = <Button onClick={close}>{closeText}</Button>;

    return (
        <span>
            <Modal show={show} onHide={close}>
                <Modal.Header>
                    <Modal.Title>{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {children}
                </Modal.Body>
                <Modal.Footer>
                    {closeButton}
                    {saveButton}
                </Modal.Footer>
            </Modal>
        </span>
    );
};

export default ModalDialog;