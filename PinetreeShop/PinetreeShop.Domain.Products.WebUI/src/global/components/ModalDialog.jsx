import { Modal, Button } from 'react-bootstrap';
import React from 'react';

class ModalDialog extends React.Component {
    constructor(props){
        super(props);
        this.close = this.close.bind(this);
        this.save = this.save.bind(this);
    }

    close() {
        if (this.props.onClose()) {
            this.props.onClose();
        }
    }

    save() {        
        if (this.props.onSave) {
            this.props.onSave();
        }
    }

    render() {
        let saveButton = null;
        if(this.props.onSave) {
            let saveText = this.props.saveText ? this.props.saveText : 'Save';
            saveButton = <Button bsStyle='primary' onClick={this.save}>{saveText}</Button>;
        }

        let cancelText = this.props.cancelText || 'Close';
        let cancelButton = <Button onClick={this.close}>{cancelText}</Button>;

        return (
            <span>
                <Modal show={this.props.show} onHide={this.close}>
                    <Modal.Header>
                        <Modal.Title>{this.props.title}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        {this.props.children}
                    </Modal.Body>
                    <Modal.Footer>
                        {cancelButton}
                        {saveButton}
                    </Modal.Footer>
                </Modal>
            </span>
        );
    }
}

export default ModalDialog;