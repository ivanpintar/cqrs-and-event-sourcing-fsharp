import React from 'react';
import { FormControl } from 'react-bootstrap';
import ModalDialog from '../../global/components/ModalDialog';

class UpdateModal extends React.Component{
    constructor(props) {
        super(props);
        this.handleChange = this.handleChange.bind(this);
        this.submit = this.submit.bind(this);
        this.state = { 
            value: 0
        }; 
    }

    componentWillReceiveProps(props) {
        this.state = { 
            value: props.product ? props.product.quantity : 0 
        }; 
    }

    handleChange(e) {
        this.setState({ value: parseInt(e.target.value, 10) });
    }

    submit() {
        if(this.props.product && this.props.product.quantity !== this.state.value) {
            let currentQuantity = this.props.product.quantity;
            let diff = this.state.value - currentQuantity;
            console.log("changing quantity " + diff)

            this.props.changeQuantity(this.props.product.id, diff);
        }
        this.props.onClose();
    }

    render() {
        let title = '';
        if(this.props.product) {
            title = 'Change quantity of: ' + this.props.product.name 
        }
        return (
            <ModalDialog 
                show={this.props.show}
                onClose={this.props.onClose}
                onSave={this.submit}
                title={title}>
                <FormControl
                    value={this.state.value}
                    type="text"
                    onChange={this.handleChange}/>     
            </ModalDialog>
        );
    }
}

export default UpdateModal;