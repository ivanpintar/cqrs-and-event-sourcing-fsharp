import React from 'react';
import { FormControl, FormGroup, ControlLabel } from 'react-bootstrap'
import ModalDialog from '../../global/components/ModalDialog';

class CheckOutModal extends React.Component{
    constructor(props) {
        super(props);
        this.handleChange = this.handleChange.bind(this);
        this.submit = this.submit.bind(this);
        this.close = this.close.bind(this);
        this.state = {
            address: ''
        }; 
    }

    handleChange(e, field) {
        var update = {};
        update[field] = e.target.value;
        this.setState(update);
    }

    close() {
        this.props.onClose();
    }

    submit() {    
        if(this.state.address) {
            this.props.checkOutBasket(this.state.address);
        }
        this.props.onClose();    
    }

    render() {
        return (
            <ModalDialog 
                show={this.props.show}
                onClose={this.props.onClose}
                onSave={this.submit}
                title='Check Out'>
				<FormGroup>
                    <ControlLabel>Address</ControlLabel>
					<FormControl
						value={this.state.address}
						type="textarea"
						onChange={(e) => this.handleChange(e, 'address')}/>
                </FormGroup>
            </ModalDialog>
        );
    }
}

export default CheckOutModal;